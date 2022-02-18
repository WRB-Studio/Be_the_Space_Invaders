using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    private Sprite spriteSoundOn;
    private Sprite spriteSoundOff;

    private static Helper Instance;
    public static Helper instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject newinstance = new GameObject("Helper");
                Instance = newinstance.AddComponent<Helper>();
                newinstance.transform.parent = MyUtilities.createAndSetToScriptFolder(false).transform;
            }

            return Instance;
        }
    }



    private void Awake()
    {
        if (transform.name == "btSound")
        {
            if (StaticAudioHandler.getSoundIsOn())
                transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/SoundOn");
            else
                transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/SoundOff");
        }

        if (transform.name == "btMusic")
        {
            if (StaticAudioHandler.getMusicIsOn())
                transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/MusicOn");
            else
                transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/MusicOff");
        }
    }


    public void formationMenueInfo()
    {
        PopupHandler.instance.showHeaderOneButtonPopup("Help", "Here you can buy, upgrade and place invaders" +
                                                               "\ninto the formation by drag & drop." +
                                                               "\nYou can also buy additional formation slots here.");
    }

    public void ingameInfo()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasTutorial"));
    }

    public void soundOnOff()
    {
        if (StaticAudioHandler.switchSoundsOnOff())
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/SoundOn");
        else
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/SoundOff");
    }

    public void musicOnOff()
    {
        if (StaticAudioHandler.switchMusicOnOff())
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/MusicOn");
        else
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/UI/Symbols/MusicOff");
    }

    public void onBtBugReport()
    {
        PopupHandler.instance.showReportBugPopup();
    }

}
