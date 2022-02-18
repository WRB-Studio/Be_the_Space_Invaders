using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class IngameInvaderHandler : MonoBehaviour
{
    public float buildUpTime = 0.1f;

    [Header("Formation layout")]
    public Transform formationLayoutParent;
    public GameObject invaderSlotRowPrefab;
    public GameObject invaderSlotPrefab;

    private GameObject[,] instantiatedUIInvaderPlaces;
    private List<GameObject> instantiatedInvader;

    [Header("Animations")]
    public GameObject hyperdrivePrefab;
    public GameObject borderAlarmGO;

    public bool allInvadersLoaded = false;

    public static IngameInvaderHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StatsHandler.instance.saveBackupResourcesForCurrentGame();

        borderAlarmGO.SetActive(false);
        createFormationLayoutUI();

        loadFormation();
    }

    private void FixedUpdate()
    {
        if (instantiatedInvader == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        bool showBorderAlarm = false;

        for (int i = 0; i < instantiatedInvader.Count; i++)
        {
            Invader invaderScrp = instantiatedInvader[i].GetComponent<Invader>();

            invaderScrp.updateCall();

            if (invaderScrp.currentHitpoints > 0 && invaderScrp.hitpointsCurrentLevelDefault > 0)
            {
                float hpPercentage = invaderScrp.currentHitpoints / (invaderScrp.hitpointsCurrentLevelDefault / 100f);
                if (hpPercentage < 20)
                    showBorderAlarm = true;
            }
        }

        if (showBorderAlarm)
            borderAlarmGO.SetActive(true);
        else
            borderAlarmGO.SetActive(false);
    }


    private void createFormationLayoutUI()
    {
        instantiatedUIInvaderPlaces = new GameObject[FormationHandler.maxRows, FormationHandler.maxColumns];

        formationLayoutParent.GetComponent<VerticalLayoutGroup>().spacing = 25;

        for (int row = 0; row < FormationHandler.maxRows; row++)
        {
            GameObject newRow = Instantiate(invaderSlotRowPrefab, formationLayoutParent);
            newRow.name = "Row " + row;

            for (int column = 0; column < FormationHandler.maxColumns; column++)
            {
                GameObject newInvaderColumnPlace = Instantiate(invaderSlotPrefab, newRow.transform);
                newInvaderColumnPlace.name = "place [" + row + ", " + column + "]";
                instantiatedUIInvaderPlaces[row, column] = newInvaderColumnPlace;
                newInvaderColumnPlace.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    private void loadFormation()
    {
        formationLayoutParent.GetComponent<RectTransform>().ForceUpdateRectTransforms();
        List<int[]> invadersInformation = new List<int[]>();//[0] = id; [1] = row; [2] = column

        for (int row = 0; row < FormationHandler.maxRows; row++)
        {
            for (int column = 0; column < FormationHandler.maxColumns; column++)
            {
                if (FormationHandler.instance.formation[row, column] > -1)
                {
                    invadersInformation.Add(new int[] { FormationHandler.instance.formation[row, column], row, column });
                }
            }
        }

        StartCoroutine(spawnInvadersFromHyperdrive(invadersInformation));
    }


    private IEnumerator spawnInvadersFromHyperdrive(List<int[]> invadersInformation)
    {
        yield return null;

        /*GameObject invaderPlanet = GameObject.Find("InvaderPlanet");
        invaderPlanet.GetComponent<Animator>().Play("PlanetHyperdriveAnim");
        yield return new WaitForSeconds(1.7f);
        Destroy(invaderPlanet);*/

        invadersInformation = (List<int[]>)MyUtilities.shuffleList(invadersInformation);

        for (int i = 0; i < invadersInformation.Count; i++)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.2f));

            int invaderID = invadersInformation[i][0];
            int[] invaderPlace = { invadersInformation[i][1], invadersInformation[i][2] };

            //Create hyperdrive
            GameObject invaderHyperDrive = Instantiate(hyperdrivePrefab);
            Vector3 hyperdrivePosition = Camera.main.ScreenToWorldPoint(instantiatedUIInvaderPlaces[invaderPlace[0], invaderPlace[1]].transform.position);
            hyperdrivePosition.z = 0;
            invaderHyperDrive.transform.position = hyperdrivePosition;
            Destroy(invaderHyperDrive, 6);
            StaticAudioHandler.playSound(SoundChooser.instance.hyperdriveInvaders);

            StartCoroutine(waitForHyperdrive(invaderID, invaderPlace));
        }

        while (instantiatedInvader == null)
            yield return null;

        while (instantiatedInvader.Count < invadersInformation.Count)
            yield return null;

        allInvadersLoaded = true;

        TerranShipHandler.instance.startNextStage();
    }

    private IEnumerator waitForHyperdrive(int id, int[] place)
    {
        yield return new WaitForSeconds(0.5f);

        createInvaderOnPlace(id, place);
    }

    private void createInvaderOnPlace(int id, int[] place)
    {
        if (instantiatedInvader == null)
            instantiatedInvader = new List<GameObject>();

        GameObject newInvader = Instantiate(InvaderLoader.instance.getLoadedInvaderWithID(id), transform);
        newInvader.name = newInvader.name.Replace("(Clone)", "");

        Vector3 invaderPos = Camera.main.ScreenToWorldPoint(instantiatedUIInvaderPlaces[place[0], place[1]].transform.position);
        invaderPos.z = 0;
        newInvader.transform.position = invaderPos;

        //set sorting order (for damage particle)
        List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
        for (int i = 0; i < instantiatedInvader.Count; i++)
            spriteRendererList.Add(instantiatedInvader[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
        MyUtilities.CalculateAndSetSortingOrder(spriteRendererList, newInvader.transform.GetChild(0).GetComponent<SpriteRenderer>(), new int[] { 0, 99 });

        newInvader.GetComponent<Invader>().init(false);

        instantiatedInvader.Add(newInvader);
    }


    public List<GameObject> getInstantiatedInvaders()
    {
        return instantiatedInvader;
    }

    public List<GameObject> getInstantiatedInvadersFromClass(Invader.Invaderclass invaderClass)
    {
        List<GameObject> filteredList = getInstantiatedInvaders();
        for (int i = 0; i < filteredList.Count; i++)
        {
            if (filteredList[i].GetComponent<Invader>().invaderClass != invaderClass)
                filteredList.Remove(filteredList[i]);
        }

        if (filteredList.Count > 0)
            return filteredList;

        return null;
    }

    public void removeInvaderFromList(GameObject removingObject)
    {
        instantiatedInvader.Remove(removingObject);
        Destroy(removingObject);

        cameraShake();

        if (instantiatedInvader.Count <= 0)
            GameOverHandler.instance.showGameOver();
    }


    public void cameraShake()
    {
        Camera.main.GetComponent<Animator>().Play("CameraShake");
    }

}