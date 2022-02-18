using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TerranShipHandler : MonoBehaviour
{
    [Header("Test settings")]
    public bool enableSpawning = true;
    public GameObject spawnSpecificShip = null;
    
    [Header("Animations")]
    public GameObject hyperdrivePrefab;

    [Header("Spawn settings")]    
    public int spawnListIndex = -1;
    public List<TerranSpawn> spawnListByLevel;

    private List<GameObject> terranShipPrefabs;
    private List<GameObject> instantiatedTerranShips;

    private bool isInit = false;
    private bool shipSpawningInProgress = false;

    public static TerranShipHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public void init()
    {               
        if (StageHandler.instance.startStage > 1)
        {
            for (int i = 0; i < StageHandler.instance.currentStage; i++)
            {
                spawnListIndex++;

                if (spawnListIndex >= spawnListByLevel.Count)
                    spawnListIndex = 0;
            }
        }

        spawnListByLevel = spawnListByLevel.OrderBy(o => o.level).ToList();//sort by level
        terranShipPrefabs = new List<GameObject>();
        MyUtilities.addListToList(terranShipPrefabs, Resources.LoadAll<GameObject>("Prefabs/TerranShips/SmallShips").ToList());
        MyUtilities.addListToList(terranShipPrefabs, Resources.LoadAll<GameObject>("Prefabs/TerranShips/Frigates").ToList());
        MyUtilities.addListToList(terranShipPrefabs, Resources.LoadAll<GameObject>("Prefabs/TerranShips/Battleships").ToList());
        terranShipPrefabs = terranShipPrefabs.OrderBy(o => o.GetComponent<TerranShip>().id).ToList();//sort by id
        instantiatedTerranShips = new List<GameObject>();

        isInit = true;
    }

    private void FixedUpdate()
    {
        if (!IngameInvaderHandler.instance.allInvadersLoaded || !isInit || IngameHandler.instance.isPauseOrGameOver() || TerranStationHandler.instance.stationAttackInProgress)
            return;

        if (instantiatedTerranShips.Count > 0)
        {
            for (int i = 0; i < instantiatedTerranShips.Count; i++)
            {
                if (instantiatedTerranShips[i] == null)
                    continue;

                instantiatedTerranShips[i].GetComponent<TerranShip>().updateCall();
            }
        }
    }


    public void startNextStage()
    {
        if (!enableSpawning)
            return;

        shipSpawningInProgress = true;

        StageHandler.instance.currentStage++;
        spawnListIndex++;

        if (spawnListIndex >= spawnListByLevel.Count) // loops the spawn list
            spawnListIndex = 0;

        StartCoroutine(spawnNextShipsCoroutine());

        //show stage popup
        PopupHandler.instance.showShortInfoPopupRefreshCurrentWhenMultiple("Stage " + StageHandler.instance.currentStage);
        IngameHandler.instance.refreshTxtStage();

        //speed up background stars
        var ps_bgStars = GameObject.Find("PS_BGStars").GetComponent<ParticleSystem>().main;
        ps_bgStars.simulationSpeed = StageHandler.instance.currentStage / 10 + 1;


        //set stage infos
        string shipnames = "";
        for (int i = 0; i < instantiatedTerranShips.Count; i++)
        {
            shipnames += "\n\t" + instantiatedTerranShips[i].name.Replace("(Clone)", "");
        }

        //show level infos
        StageHandler.instance.txtStageInfo.text = "Stage: " + StageHandler.instance.currentStage + "\nShips: " + shipnames;
    }

    private IEnumerator spawnNextShipsCoroutine()
    {
        yield return null;

        //When a specific ship is to spawn (for testing)
        if (spawnSpecificShip != null)
        {
            //hyperdrive instantiation
            GameObject terranHyperDrive = Instantiate(hyperdrivePrefab);
            Vector3 displayDimension = MyUtilities.getOrthographicCameraBounds(Camera.main).extents;
            terranHyperDrive.transform.position = new Vector3(Random.Range(-displayDimension.x + 0.5f, displayDimension.x - 0.5f),
                                                              Random.Range(-displayDimension.y - 1.5f, -displayDimension.y + 1f),
                                                              0);
            Destroy(terranHyperDrive, 6);
            StaticAudioHandler.playSound(SoundChooser.instance.hyperdriveTerrans);

            //terran ship spawn
            Vector3 spawnPosition = terranHyperDrive.transform.position;
            spawnPosition.y += 2;
            yield return new WaitForSeconds(0.5f);
            instantiateShip(spawnSpecificShip, spawnPosition);
        }
        else //spawn ships from spawn list
        {
            //spawn list
            TerranSpawn currentTerranSpawnList = spawnListByLevel[spawnListIndex];

            for (int i = 0; i < currentTerranSpawnList.spawnshipID.Length; i++)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.6f)); // random shortly delay for spawning

                //hyperdrive instantiation
                GameObject terranHyperDrive = Instantiate(hyperdrivePrefab);
                Vector3 displayDimension = MyUtilities.getOrthographicCameraBounds(Camera.main).extents;
                terranHyperDrive.transform.position = new Vector3(Random.Range(-displayDimension.x + 0.5f, displayDimension.x - 0.5f),
                                                              Random.Range(-displayDimension.y - 1.8f, -displayDimension.y - 0.8f),
                                                              0);
                Destroy(terranHyperDrive, 6);
                StaticAudioHandler.playSound(SoundChooser.instance.hyperdriveTerrans);

                //terran ship spawn
                Vector3 spawnPosition = terranHyperDrive.transform.position;
                spawnPosition.y += Random.Range(1.75f, 2.75f);
                yield return new WaitForSeconds(0.5f);
                instantiateShip(getTerranShipByID(currentTerranSpawnList.spawnshipID[i]), spawnPosition);
            }
        }

        shipSpawningInProgress = false;
    }

    public void instantiateShip(GameObject terranShipPrefab, Vector3 spawnPosition)
    {
        //instantiate terran ship
        GameObject newTerranShip = Instantiate(terranShipPrefab, transform);

        //set random spawn position
        newTerranShip.transform.position = spawnPosition;

        //set sorting order (for damage particle)
        List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
        for (int i = 0; i < instantiatedTerranShips.Count; i++)
            spriteRendererList.Add(instantiatedTerranShips[i].GetComponent<SpriteRenderer>());
        MyUtilities.CalculateAndSetSortingOrder(spriteRendererList, newTerranShip.GetComponent<SpriteRenderer>(), new int[] { 0, 99 });

        instantiatedTerranShips.Add(newTerranShip);

        //init terran ship
        newTerranShip.GetComponent<TerranShip>().init(StageHandler.instance.currentStage);
    }

    private GameObject getTerranShipByID(int id)
    {
        for (int i = 0; i < terranShipPrefabs.Count; i++)
        {
            if (terranShipPrefabs[i].GetComponent<TerranShip>().id == id)
                return terranShipPrefabs[i];
        }

        return null;
    }


    public List<GameObject> getInstantiatedTerranShips()
    {
        return instantiatedTerranShips;
    }

    public void removeInvaderFromList(GameObject removingObject)
    {
        MyUtilities.removeAndDestroyGameObjectFromList(ref instantiatedTerranShips, removingObject);

        if (instantiatedTerranShips.Count <= 0 && !shipSpawningInProgress)
            checkNextStageSpawn();
    }

    private void checkNextStageSpawn()
    {
        //if no terrans exist and no one is currently spawning
        if (!TerranStationHandler.instance.stationAttackInProgress)
        {
            //spawn station every 10th stage
            if (StageHandler.instance.currentStage > 0 && MyUtilities.isMultipleOf(StageHandler.instance.currentStage, TerranStationHandler.instance.spawnMultiplieOf))
            {
                TerranStationHandler.instance.spawnStation();
            }
            else
            {
                startNextStage();
            }
        }
    }

}
