using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public string lastSavegameWipeVersion;
    public bool formationWipe = false;
    public static string staticLastSavegameWipeVersion;
    public static bool staticFormationWipe;
    public static GameObject canvasLoadingBarPrefab;
    public static GameObject instantiatedCanvasLoadingBar;
    public static Vector3 invaderPlanetStartPosition;

    public static GameHandler instance;
    public static bool isInit = false;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        invaderPlanetStartPosition = GameObject.Find("InvaderPlanet").transform.position;
    }

    public static void resetInvaderPlanet()
    {
        GameObject.Find("InvaderPlanet").transform.position = invaderPlanetStartPosition;
    }

    public void init()
    {
        canvasLoadingBarPrefab = Resources.Load<GameObject>("Prefabs/UI/CanvasLoadingBar");
        staticLastSavegameWipeVersion = lastSavegameWipeVersion;

        showHideLoadingProgressbar(true);

        GameObject.Find("GPGS").GetComponent<PlayCloudDataManager>().initAndLoad(delegate () { initScripts(); });
    }

    private void initScripts()
    {
        FPSScrp.instance.init();
        StaticAudioHandler.instance.init();

        StatsHandler.instance.init();
        FormationHandler.instance.init();
        InvaderLoader.instance.init();

        showHideLoadingProgressbar(false);

        isInit = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            askBeforeExitGame();
        }
    }


    public void showHideLoadingProgressbar(bool show)
    {
        if (show)
        {
            if (instantiatedCanvasLoadingBar != null && instantiatedCanvasLoadingBar.activeSelf)
            {
                instantiatedCanvasLoadingBar.SetActive(true);
            }
            else
            {
                instantiatedCanvasLoadingBar = Instantiate(canvasLoadingBarPrefab);
                instantiatedCanvasLoadingBar.transform.SetSiblingIndex(1);
            }
        }
        else
        {
            if (instantiatedCanvasLoadingBar != null)
                Destroy(instantiatedCanvasLoadingBar);
        }
    }


    public void showMainMenue(bool appReset = false)
    {
        resetInvaderPlanet();

        if(!appReset)
            SaveLoadSystem.instance.saveSavegame();

        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            StaticAudioHandler.playSound(SoundChooser.instance.sceneSwitch);
            if (appReset)
                Destroy(transform.parent.gameObject);
        }));
    }

    public void showIngame()
    {
        resetInvaderPlanet();

        StatsHandler.instance.resetCurrentGameStats();

        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
            SceneManager.LoadScene("Ingame", LoadSceneMode.Single);
            StaticAudioHandler.playSound(SoundChooser.instance.sceneSwitch);
        }));
    }

    public void showFormationMenue()
    {
        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
            SceneManager.LoadScene("FormationMenu", LoadSceneMode.Single);
            StaticAudioHandler.playSound(SoundChooser.instance.sceneSwitch);
        }));
    }

    public void showStatsMenue()
    {
        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
            SceneManager.LoadScene("StatsMenu", LoadSceneMode.Single);
            StaticAudioHandler.playSound(SoundChooser.instance.sceneSwitch);
        }));
    }

    public void showSettingsMenue()
    {
        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
            SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Single);
            StaticAudioHandler.playSound(SoundChooser.instance.sceneSwitch);
        }));
    }

    public void instantExitGame()
    {
        instance.StartCoroutine(waitForSaveOrLoadingProgress(delegate () {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }));


    }

    public void askBeforeExitGame()
    {
        if (IngameHandler.instance != null)
            IngameHandler.instance.isPause = true;

        PopupHandler.instance.showPopupWithTwoButton("Quit game?",
                                                     "Yes", delegate () { instantExitGame(); },
                                                     "No", delegate ()
                                                            {
                                                                if (IngameHandler.instance != null)
                                                                    IngameHandler.instance.isPause = false;
                                                            });
    }

    public IEnumerator waitForSaveOrLoadingProgress(MyUtilities.DelegateMethode_1 onProgressFinished)
    {
        /*while (SaveLoadSystem.instance.isLoading || SaveLoadSystem.instance.isSaving)
            yield return null;*/

        yield return null;
        onProgressFinished();
    }

}
