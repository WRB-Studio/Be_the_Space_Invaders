using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [Header("UI references")]
    public GameObject gameOverGO;
    public Text txtPlayedTime;
    public Text txtLevel;
    public Text txtTerranKills;
    public Text txtStationKills;
    public Text txtAsteroidKills;
    public Text txtCollectedResources;
    public Text txtBonusResources;
    public Text txtTotalResources;
    public Text txtMyResources;
    public Text txtTotalScore;
    public Text txtBestScore;

    public Button btReplay;
    public Button btMainMenu;
    public Button btAd;

    [Header("Visuel settings")]
    public float textCountAnimDuration = 2f;

    [Header("Score settings")]
    public uint extraScoreMultiplier = 25;

    private bool scoreCoroutineInProgress = false;

    private uint tmpOldTotalResources;
    private uint tmpCollectedResources;
    private uint tmpBonusRes;
    private uint tmpTotalGainedResources;
    private uint tmpTotalScore;
    private Coroutine counterCoroutine;

    public static GameOverHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        btReplay.onClick.AddListener(() => { GameHandler.instance.showIngame(); });
        btMainMenu.onClick.AddListener(() => { GameHandler.instance.showMainMenue(); });
        btAd.onClick.AddListener(() => { StartCoroutine(adClickCoroutine()); });
    }


    public void showGameOver()
    {
        if (StaticAudioHandler.getAudioSrcMusic() != null)
            Destroy(StaticAudioHandler.getAudioSrcMusic().gameObject);

        StaticAudioHandler.playSound(SoundChooser.instance.gameOver);
        StaticAudioHandler.playMusic(SoundChooser.instance.gameOver_music);

        btReplay.interactable = false;
        btMainMenu.interactable = false;
        btAd.interactable = false;

        //time
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(StatsHandler.instance.CurrentPlayedTime);
        txtPlayedTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        //score calculation
        tmpTotalScore = StatsHandler.instance.CurrentScore;
        if (StatsHandler.instance.CurrentScore > StatsHandler.instance.Highscore)
        {
            StatsHandler.instance.checkNewHighscore(StatsHandler.instance.CurrentScore);
            PlayCloudDataManager.instance.postScoreToLeaderBoard(StatsHandler.instance.Highscore);
        }

        //resource calculations
        tmpOldTotalResources = StatsHandler.instance.BackupResourcesForCurrentGame;
        tmpCollectedResources = StatsHandler.instance.CurrentCollectedResources;
        tmpBonusRes = (uint)Mathf.RoundToInt(tmpTotalScore / extraScoreMultiplier);
        tmpTotalGainedResources = tmpCollectedResources + tmpBonusRes;
        StatsHandler.instance.addResources(tmpBonusRes);

        //set start texts
        txtLevel.text = "Stage " + StageHandler.instance.currentStage;
        txtTotalScore.text = "0";
        txtBestScore.text = StatsHandler.instance.Highscore.ToString();
        txtTerranKills.text = "0";
        txtStationKills.text = "0";
        txtAsteroidKills.text = "0";
        txtCollectedResources.text = "0";
        txtBonusResources.text = "0";
        txtTotalResources.text = "0";
        txtMyResources.text = tmpOldTotalResources.ToString();
        
        gameOverGO.SetActive(true);

        IngameHandler.instance.isGameOver = true;

        if (tmpTotalScore != 0)
            counterCoroutine = StartCoroutine(scoreCounterCoroutine());
        else
            showScoreInstant();

        SaveLoadSystem.instance.saveSavegame();
    }

    private void showScoreInstant()
    {
        //total score
        txtTotalScore.text = tmpTotalScore.ToString();

        //best score
        if (StatsHandler.instance.CurrentScore > StatsHandler.instance.Highscore)
            PopupHandler.instance.showShortInfoPopup("New Highscore", PopupHandler.PopupAlignment.Center);
        txtBestScore.text = StatsHandler.instance.Highscore.ToString();

        //Terran kills, Asteroid kills
        txtTerranKills.text = StatsHandler.instance.CurrentKilledTerrans.ToString();
        txtStationKills.text = StatsHandler.instance.CurrentKilledStations.ToString();
        txtAsteroidKills.text = StatsHandler.instance.CurrentKilledAsteroids.ToString();

        //Resources gain
        txtCollectedResources.text = tmpCollectedResources.ToString();
        txtBonusResources.text = tmpBonusRes.ToString();
        txtTotalResources.text = tmpTotalGainedResources.ToString();
        txtMyResources.text = StatsHandler.instance.Resources.ToString();

        btReplay.interactable = true;
        btMainMenu.interactable = true;

        if (tmpTotalGainedResources > 0)
            btAd.interactable = true;
    }


    private IEnumerator scoreCounterCoroutine()
    {
        scoreCoroutineInProgress = true;

        yield return null;

        StartCoroutine(cancelCoroutineByClick());

        //total score
        MyUtilities.instance.textCountAnimation(tmpTotalScore, textCountAnimDuration, txtTotalScore, true);
        yield return new WaitForSeconds(0.3f);
        //best score
        if (StatsHandler.instance.CurrentScore > StatsHandler.instance.Highscore)
            PopupHandler.instance.showShortInfoPopup("New Highscore", PopupHandler.PopupAlignment.Center);
        txtBestScore.text = StatsHandler.instance.Highscore.ToString();
        yield return new WaitForSeconds(0.5f);

        //Terran kills, Asteroid kills
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.CurrentKilledTerrans, textCountAnimDuration, txtTerranKills, true);
        yield return new WaitForSeconds(0.1f);
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.CurrentKilledStations, textCountAnimDuration, txtStationKills, true);
        yield return new WaitForSeconds(0.1f);
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.CurrentKilledAsteroids, textCountAnimDuration, txtAsteroidKills, true);
        yield return new WaitForSeconds(0.5f);

        //resources
        MyUtilities.instance.textCountAnimation(tmpCollectedResources, textCountAnimDuration, txtCollectedResources, true);
        yield return new WaitForSeconds(0.3f);        

        if (tmpBonusRes > 0)
        {
            MyUtilities.instance.textCountAnimation(tmpBonusRes, textCountAnimDuration, txtBonusResources, true);
            yield return new WaitForSeconds(0.3f);
        }
        MyUtilities.instance.textCountAnimation(tmpTotalGainedResources, textCountAnimDuration, txtTotalResources, true);
        yield return new WaitForSeconds(0.3f);
        MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, textCountAnimDuration, txtMyResources, true);
        yield return new WaitForSeconds(0.5f);

        btReplay.interactable = true;
        btMainMenu.interactable = true;
        if (tmpTotalGainedResources > 0)
            btAd.interactable = true;

        scoreCoroutineInProgress = false;
    }

    private IEnumerator cancelCoroutineByClick()
    {
        while (scoreCoroutineInProgress)
        {
            yield return null;
            if (Input.GetMouseButton(0))
            {
                scoreCoroutineInProgress = false;
                StopCoroutine(counterCoroutine);

                MyUtilities.instance.stopTextCountAnimation();

                yield return new WaitForSeconds(0.1f);
                showScoreInstant();
            }
        }

    }

    private IEnumerator adClickCoroutine()
    {
        yield return null;

        PopupHandler.instance.showPopupWithOneButton("Advertising will be implemented soon!", "OK", delegate() {

            StatsHandler.instance.addResources(tmpTotalGainedResources);
            SaveLoadSystem.instance.saveSavegame();

            MyUtilities.instance.textCountAnimation(tmpTotalGainedResources * 2, textCountAnimDuration, txtTotalResources, true);
            MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, textCountAnimDuration, txtMyResources, true);

            txtMyResources.text = StatsHandler.instance.Resources.ToString();

        });        

        btAd.interactable = false;
    }

}
