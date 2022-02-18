using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScrp : MonoBehaviour
{
    public static DontDestroyOnLoadScrp instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GameObject.Find("GameHandler").GetComponent<GameHandler>().init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
