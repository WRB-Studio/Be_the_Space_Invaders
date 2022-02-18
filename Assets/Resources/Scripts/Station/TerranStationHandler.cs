using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerranStationHandler : MonoBehaviour
{
    [Header("General settings")]
    public float bgSpeedChangeDuration = 3f;
    private float tmpBgStarsSpeed;
    public Text txtStageInfo;
    public bool showStageInfos = false;

    [Header("Spawn settings")]
    public int spawnMultiplieOf = 10;
    public int spawnListIndex = -1;
    public GameObject spawnSpecificStation = null;
    public GameObject[] stationPrefabs;
    private GameObject instantiatedStation;
    public bool stationAttackInProgress { get; set; }

    private ParticleSystem.MainModule ps_bgStars;

    public Slider sliderHitpointsStation;

    public static TerranStationHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sliderHitpointsStation.gameObject.SetActive(false);

        //stationPrefabs = Resources.LoadAll<GameObject>("Prefabs/Stations");
    }

    private void FixedUpdate()
    {
        if (instantiatedStation == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        instantiatedStation.GetComponent<TerranStation>().updateCall();
    }


    public void spawnStation()
    {
        stationAttackInProgress = true;

        //show station popup
        int tmpSpawnListIndex = spawnListIndex + 1;
        if (tmpSpawnListIndex >= stationPrefabs.Length)
            tmpSpawnListIndex = 0;
        string stationName = stationPrefabs[tmpSpawnListIndex].name;
        PopupHandler.instance.showShortInfoPopupRefreshCurrentWhenMultiple(stationName + " reached");
        IngameHandler.instance.refreshTxtStage();

        //save background stars simulation speed
        ps_bgStars = GameObject.Find("PS_BGStars").GetComponent<ParticleSystem>().main;
        tmpBgStarsSpeed = ps_bgStars.simulationSpeed;

        StartCoroutine(spawnStationCoroutine());
    }

    public void instantiateStation()
    {
        spawnListIndex++;
        if (spawnListIndex >= stationPrefabs.Length)
            spawnListIndex = 0;

        //set random spawn position
        Vector3 newPosition = Vector3.zero;
        newPosition.y = -MyUtilities.getDisplayDimension(Camera.main).y - tmpBgStarsSpeed;

        GameObject newStation;
        if (spawnSpecificStation != null)
        {
            newStation = Instantiate(spawnSpecificStation, newPosition, spawnSpecificStation.transform.rotation, transform);
        }
        else
        {
            newStation = Instantiate(stationPrefabs[spawnListIndex], newPosition, stationPrefabs[spawnListIndex].transform.rotation, transform);
        }

        //set sorting order (for damage particle)
        List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
        spriteRendererList.Add(newStation.GetComponent<SpriteRenderer>());
        MyUtilities.CalculateAndSetSortingOrder(spriteRendererList, newStation.GetComponent<SpriteRenderer>(), new int[] { 0, 99 });

        newStation.GetComponent<TerranStation>().calculateStationStats(StageHandler.instance.currentStage);

        instantiatedStation = newStation;
    }

    public void removeStationFromList(GameObject removingObject)
    {
        sliderHitpointsStation.gameObject.SetActive(false);
        Destroy(instantiatedStation);

        ShootHandler.instance.refreshMissileTargetsByDestroyedStation();

        StartCoroutine(stationDestroyedCoroutine());
    }

    public GameObject getInstantiatedTerranStation()
    {
        return instantiatedStation;
    }


    private IEnumerator spawnStationCoroutine()
    {
        yield return new WaitForSeconds(1);

        instantiateStation();

        StartCoroutine(HPBarFillCoroutine());

        Vector3 moveTo = Vector3.zero;
        moveTo.y = -MyUtilities.getDisplayDimension(Camera.main).y + 1.5f;

        float startYPos = instantiatedStation.transform.position.y;

        float currentDurationTime = 0;
        float updateValue;
        while (Vector3.Distance(instantiatedStation.transform.position, moveTo) > 0.15f)
        {
            currentDurationTime += Time.fixedDeltaTime / bgSpeedChangeDuration;

            updateValue = Mathf.Lerp(startYPos, moveTo.y, currentDurationTime);
            instantiatedStation.transform.position = new Vector3(instantiatedStation.transform.position.x,
                                                     updateValue,
                                                     instantiatedStation.transform.position.z);

            updateValue = Mathf.SmoothStep(tmpBgStarsSpeed, 0, currentDurationTime / 2);
            ps_bgStars.simulationSpeed = updateValue;

            yield return null;
        }
        ps_bgStars.simulationSpeed = 0;
    }

    private IEnumerator HPBarFillCoroutine()
    {
        yield return null;
        sliderHitpointsStation.value = 0;
        sliderHitpointsStation.gameObject.SetActive(true);

        while (instantiatedStation.GetComponent<TerranStation>().getCurrentHitpoints() == 0)
            yield return null;

        sliderHitpointsStation.maxValue = instantiatedStation.GetComponent<TerranStation>().hitpoints;

        StaticAudioHandler.playSound(SoundChooser.instance.fillStationHPBar);

        float animationDuration = 2;
        int start = 0;
        for (float timer = 0; timer < animationDuration; timer += Time.deltaTime)
        {
            float progress = timer / animationDuration;
            sliderHitpointsStation.value = (uint)Mathf.Lerp(start, sliderHitpointsStation.maxValue, progress);
            yield return null;
        }

        sliderHitpointsStation.value = sliderHitpointsStation.maxValue;

        instantiatedStation.GetComponent<TerranStation>().init();
    }

    private IEnumerator stationDestroyedCoroutine()
    {
        yield return null;

        //speed up background stars

        float currentDurationTime = 0;
        float updateValue;
        while (ps_bgStars.simulationSpeed < tmpBgStarsSpeed)
        {
            currentDurationTime += Time.fixedDeltaTime / bgSpeedChangeDuration;

            updateValue = Mathf.SmoothStep(0, tmpBgStarsSpeed, currentDurationTime);
            ps_bgStars.simulationSpeed = updateValue;

            yield return null;
        }
        ps_bgStars.simulationSpeed = 0;

        stationAttackInProgress = false;

        TerranShipHandler.instance.startNextStage();
    }

}
