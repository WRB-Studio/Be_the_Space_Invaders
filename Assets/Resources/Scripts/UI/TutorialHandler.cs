using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    public Color btSelectedColor;

    public Button btLayout;
    public Button btResources;
    public Button btTapShoot;
    public Button btDragShoot;
    public Button btClose;

    public GameObject layoutTutorial;
    public GameObject resourcesTutorial;
    public GameObject tapShootTutorial;
    public GameObject dragShootTutorial;

    public Text txtAnimTutorialInfo;

    public static TutorialHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        btLayout.onClick.AddListener(() => { showLayoutTutorial(); });
        btResources.onClick.AddListener(() => { showResourcesTutorial(); });
        btTapShoot.onClick.AddListener(() => { showTapShootTutorial(); });
        btDragShoot.onClick.AddListener(() => { showDragShootTutorial(); });

        btClose.onClick.AddListener(() => { close(); });

        showLayoutTutorial();
    }


    public void showLayoutTutorial()
    {
        hideAllTutorials();
        layoutTutorial.SetActive(true);
        txtAnimTutorialInfo.text = "The Invaders-hud shows the available weapons with their current cooldown and a hitpoint bar.";
        MyUtilities.changeImageColor(btLayout.gameObject, btSelectedColor);
    }

    public void showResourcesTutorial()
    {
        hideAllTutorials();
        resourcesTutorial.SetActive(true);
        txtAnimTutorialInfo.text = "Resources can be collected by tapping or swiping.\nEvery third level and every destroyed station\nincreases the resource gain.";
        MyUtilities.changeImageColor(btResources.gameObject, btSelectedColor);
    }

    public void showTapShootTutorial()
    {
        hideAllTutorials();
        tapShootTutorial.SetActive(true);
        txtAnimTutorialInfo.text = "Single, double or triple tap on Invader to shoot with different weapons.";
        MyUtilities.changeImageColor(btTapShoot.gameObject, btSelectedColor);
    }

    public void showDragShootTutorial()
    {
        hideAllTutorials();
        dragShootTutorial.SetActive(true);
        txtAnimTutorialInfo.text = "Drag & drop to manually set a target\n(terrans, asteroids, enemy shoots) for missiles\nor assign a support system to invaders.";
        MyUtilities.changeImageColor(btDragShoot.gameObject, btSelectedColor);
    }

    private void hideAllTutorials()
    {
        layoutTutorial.SetActive(false);
        resourcesTutorial.SetActive(false);
        tapShootTutorial.SetActive(false);
        dragShootTutorial.SetActive(false);
        txtAnimTutorialInfo.text = "";

        MyUtilities.changeImageColor(btLayout.gameObject, Color.black);
        MyUtilities.changeImageColor(btResources.gameObject, Color.black);
        MyUtilities.changeImageColor(btTapShoot.gameObject, Color.black);
        MyUtilities.changeImageColor(btDragShoot.gameObject, Color.black);
    }

    private void close()
    {
        if (!SaveLoadSystem.instance.getTutorialData().firstTimePlayIngameCompleted)
        {
            IngameHandler.instance.isPause = false;

            PlayCloudDataManager.currentSavegame.tutorialData.firstTimePlayIngameCompleted = true;
        }

        Destroy(gameObject);
    }


    /*-------------------Static methods----------------------*/

    public static bool firstTimeStartApplication()
    {
        if (!SaveLoadSystem.instance.getTutorialData().firstTimeStartApplicationCompleted)
        {
            PopupHandler.instance.showHeaderOneButtonPopup("Help", "First time play this game!!! =D");

            PlayCloudDataManager.currentSavegame.tutorialData.firstTimeStartApplicationCompleted = true;

            return true;
        }

        return false;
    }

    public static void startFirstIngamePlaying()
    {
        if (!SaveLoadSystem.instance.getTutorialData().firstTimePlayIngameCompleted)
        {
            IngameHandler.instance.isPause = true;

            Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasTutorial"));
        }
    }



}
