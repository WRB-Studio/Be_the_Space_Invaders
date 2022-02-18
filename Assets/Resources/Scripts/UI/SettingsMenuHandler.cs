using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuHandler : MonoBehaviour
{
    public Button btFPS;
    public Button btRemoveSavegame;
    public Button btMainMenu;



    private void Start()
    {
        if(SaveLoadSystem.instance.loadShowFPS())
            btFPS.transform.GetChild(0).GetComponent<Text>().text = "Show FPS: ON";
        else
            btFPS.transform.GetChild(0).GetComponent<Text>().text = "Show FPS: OFF";

        btFPS.onClick.AddListener(() => { onBtFPS(); });
        btRemoveSavegame.onClick.AddListener(() => { onBtRemoveSaveGame(); });
        btMainMenu.onClick.AddListener(() => { GameHandler.instance.showMainMenue(); });
    }

    private void onBtFPS()
    {
        if (GameObject.Find("FPSScrp") != null)
        {
            FPSScrp fpsScrp = GameObject.Find("FPSScrp").GetComponent<FPSScrp>();
            if (fpsScrp.showFPS)
            {
                fpsScrp.showFPS = false;
                btFPS.transform.GetChild(0).GetComponent<Text>().text = "Show FPS: OFF";
            }
            else
            {
                fpsScrp.showFPS = true;
                btFPS.transform.GetChild(0).GetComponent<Text>().text = "Show FPS: ON";
            }
            PlayCloudDataManager.currentSavegame.showFPS = fpsScrp.showFPS;
        }
    }

    private void onBtRemoveSaveGame()
    {
        PopupHandler.instance.showPopupWithTwoButton("Delete savegame?", "Yes",
            delegate ()
            {
                SaveLoadSystem.instance.removeSavegame();
            }, "No", null);
    }

}
