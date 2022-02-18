using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageHandler : MonoBehaviour
{
    public int startStage = 0;
    public int currentStage = 0;
    public Text txtStageInfo;
    public bool showStageInfos = false;

    public static StageHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (showStageInfos)
            txtStageInfo.gameObject.SetActive(true);
        else
            txtStageInfo.gameObject.SetActive(false);

        if (startStage < 1)
            startStage = 1;
        currentStage = startStage - 1;

        TerranShipHandler.instance.init();
    }

}
