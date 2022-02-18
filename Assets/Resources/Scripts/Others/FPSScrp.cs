using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSScrp : MonoBehaviour
{
    public bool showFPS { get; set; }

    public float frequency = 0.5f;
    private int FramesPerSec;
    private string fps;
    private GUIStyle guiStyle;

    public static FPSScrp instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void init()
    {
        showFPS = SaveLoadSystem.instance.loadShowFPS();

        guiStyle = new GUIStyle();
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;

        StartCoroutine(FPS());
    }


    private IEnumerator FPS()
    {
        for (; ; )
        {
            while (!showFPS) yield return null;

            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }


    void OnGUI()
    {
        if (showFPS)
        {
            GUI.Label(new Rect(Screen.width - 100, Screen.height - 50, 150, 20), fps, guiStyle);
        }
    }
}
