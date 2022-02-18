using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameHandler : MonoBehaviour
{
    public Text txtScore;
    public Text txtResources;
    public Text txtStage;

    public bool isPause;
    public bool isGameOver;

    [Header("Animations")]
    public GameObject canvasPointFeedbackPrefab;
    private bool showPointsCoroutineInProgress = false;
    public GameObject connectEffectPrefab;

    public static IngameHandler instance;




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        txtScore.text = "Score: 0";
        txtResources.text = "Resources: 0";
        refreshTxtStage();

        TutorialHandler.startFirstIngamePlaying();

        if (StaticAudioHandler.getAudioSrcMusic() != null)
            Destroy(StaticAudioHandler.getAudioSrcMusic().gameObject);
        StaticAudioHandler.playMusic(SoundChooser.instance.ingame_music);

        StartCoroutine(countPlayTimeCoroutine());

        StartCoroutine(moveInvaderPlanetUp());
    }

    private IEnumerator moveInvaderPlanetUp()
    {
        GameObject invaderPlanet = GameObject.Find("InvaderPlanet");
        while (IngameHandler.instance != null && !IngameHandler.instance.isGameOver)
        {
            if (IngameHandler.instance.isGameOver)
                break;
            while (IngameHandler.instance.isPause) yield return null;

            yield return null;

            Vector3 positionUpdate = invaderPlanet.transform.position;
            positionUpdate.y += 0.001f;
            invaderPlanet.transform.position = positionUpdate;
            if (invaderPlanet.transform.position.y > 15)
                break;
        }
    }

    public bool isPauseOrGameOver()
    {
        if (isPause || isGameOver)
            return true;
        return false;
    }


    public void refreshTxtScore(uint oldState, uint newState)
    {
        MyUtilities.instance.textAddCountAnimation(oldState, newState, 0.25f, txtScore, false, "Score: ");
    }

    public void refreshTxtResources(uint oldState, uint newState)
    {
        MyUtilities.instance.textAddCountAnimation(oldState, newState, 0.25f, txtResources, false, "Resources: ");
    }

    public void refreshTxtStage()
    {
        txtStage.text = "Stage: " + StageHandler.instance.currentStage;
    }

    public void showPointsFeedback(uint amount, Vector3 position, Color textColor)
    {
        StartCoroutine(showPointsFeedbackCoroutine(amount.ToString(), position, textColor));
    }

    public void showPointsFeedback(string info, Vector3 position, Color textColor)
    {
        StartCoroutine(showPointsFeedbackCoroutine(info, position, textColor));
    }

    private IEnumerator showPointsFeedbackCoroutine(string amount, Vector3 position, Color textColor)
    {
        while (showPointsCoroutineInProgress) yield return null;
        showPointsCoroutineInProgress = true;

        GameObject newCanvasPointFeedback = Instantiate(canvasPointFeedbackPrefab);
        newCanvasPointFeedback.transform.position = position;
        newCanvasPointFeedback.transform.GetChild(0).GetComponent<Text>().text = amount.ToString();
        newCanvasPointFeedback.transform.GetChild(0).GetComponent<Text>().color = textColor;
        Destroy(newCanvasPointFeedback, 1);
        yield return new WaitForSeconds(0.05f);

        showPointsCoroutineInProgress = false;

        /*if (toScoreEffect)
        {
            GameObject newConnectEffectPrefab = Instantiate(connectEffectPrefab);
            newConnectEffectPrefab.transform.position = newCanvasPointFeedback.transform.position;

            var currentPos = newConnectEffectPrefab.transform.position;
            var targetPos = Camera.main.ScreenToWorldPoint(txtScore.transform.position);
            targetPos.x += 0.75f;
            targetPos.y += 0.05f;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / 0.7f;
                newConnectEffectPrefab.transform.position = Vector3.Lerp(currentPos, targetPos, t);
                yield return null;
            }
            Destroy(newConnectEffectPrefab, 1);
        }*/
    }



    private IEnumerator countPlayTimeCoroutine()
    {
        while (!isGameOver)
        {
            while (isPause) yield return null;

            StatsHandler.instance.CurrentPlayedTime = 1;
            yield return new WaitForSeconds(1);
        }
    }

}
