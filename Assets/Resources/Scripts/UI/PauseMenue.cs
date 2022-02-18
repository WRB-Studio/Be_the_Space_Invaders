using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenue : MonoBehaviour
{
    public Button btPause;

    public Button btContinue;
    public Button btReplay;
    public Button btSurrender;

    public GameObject pauseMenueGO;


    public static PauseMenue instance;




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        btPause.onClick.AddListener(() => { showHidePauseMenue(); });

        btContinue.onClick.AddListener(() => { showHidePauseMenue(false); });
        btReplay.onClick.AddListener(() => { StartCoroutine(replayCoroutine()); });
        btSurrender.onClick.AddListener(() => 
        {
            showHidePauseMenue(false);
            GameOverHandler.instance.showGameOver(); 
        });

    }


    public void showHidePauseMenue(bool visible)
    {
        if (IngameHandler.instance.isGameOver)
            return;

        pauseMenueGO.SetActive(visible);
        IngameHandler.instance.isPause = visible;
    }

    private void showHidePauseMenue()
    {
        if (IngameHandler.instance.isGameOver)
            return;

        if (pauseMenueGO.activeSelf)
        {
            pauseMenueGO.SetActive(false);
            IngameHandler.instance.isPause = false;
        }
        else
        {
            pauseMenueGO.SetActive(true);
            IngameHandler.instance.isPause = true;
        }
    }

    private IEnumerator replayCoroutine()
    {
        yield return null;

        SaveLoadSystem.instance.saveSavegame();

        while (SaveLoadSystem.instance.isSaving)
            yield return null;

        GameHandler.instance.showIngame();

    }
}
