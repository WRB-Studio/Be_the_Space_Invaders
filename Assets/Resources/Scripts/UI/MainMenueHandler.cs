using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenueHandler : MonoBehaviour
{
    [Header("42 settings")]
    private int _42_counter = 0;
    public float _42_writeSpeed = 0.01f;
    public Text txt_42_;
    public GameObject _42_hint_prefab;
    private bool _42_isInit;

    [Header("UI elements")]
    public Image imgTitle;
    public Text txtResources;
    public Text txtHighScore;
    public Text txtVersion;
    public Button btStart;
    public Button btFormation;
    public Button btStats;
    public Button btSettings;
    public Button btExit;
    public Button btAchievements;
    public Button btLeaderboard;

    [Header("Develope settings")]
    public bool showDebugSettings = false;
    public Button btResAdd;
    public uint resAddAmount;
    public Button btUnlockConditionsFullfilled;

    public static MainMenueHandler instance;



    private void Awake()
    {
        instance = this;

        txtVersion.text = "v" + Application.version;
    }

    public void Start()
    {
        btResAdd.gameObject.SetActive(false);
        btUnlockConditionsFullfilled.gameObject.SetActive(false);

        btAchievements.gameObject.SetActive(false);
        btLeaderboard.gameObject.SetActive(false);

        StartCoroutine(initCoroutine());
    }

    public void init()
    {
        if (showDebugSettings)
        {
            btResAdd.gameObject.SetActive(true);
            btResAdd.onClick.AddListener(() =>
            {
                StatsHandler.instance.addResources(resAddAmount);
                MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, 0.25f, txtResources, false);
            });

            btUnlockConditionsFullfilled.gameObject.SetActive(true);
            btUnlockConditionsFullfilled.onClick.AddListener(() =>
            {
                StatsHandler.instance.hunterKills = 50;
                StatsHandler.instance.bomberKills = 30;

                StatsHandler.instance.addTotalAsteroidKills(30);
                StatsHandler.instance.scrapCrusherDefences = 60;
                StatsHandler.instance.defenderShieldUses = 80;

                StatsHandler.instance.manualCollectedResPieces = 1600;
                StatsHandler.instance.addTotalLostInvaders(160);
                for (int i = 0; i < FormationHandler.instance.maxFormationSlots; i++)
                    FormationHandler.instance.addFormationSlot();

                StatsHandler.instance.destroyerKills = 50;
                StatsHandler.instance.cruiserKills = 50;

                SaveLoadSystem.instance.saveSavegame();
            });
        }

        btStart.onClick.AddListener(() =>
        {
            if (FormationHandler.instance.isFormationEmpty())
            {
                PopupHandler.instance.showPopupWithTwoButton("There are no invaders in the formation!",
                                                             "Formation Menu", delegate () { GameHandler.instance.showFormationMenue(); },
                                                             "Close", null);
            }
            else
            {
                GameHandler.instance.showIngame();
            }
        });
        btFormation.onClick.AddListener(() => { GameHandler.instance.showFormationMenue(); });
        btStats.onClick.AddListener(() => { GameHandler.instance.showStatsMenue(); });
        btSettings.onClick.AddListener(() => { GameHandler.instance.showSettingsMenue(); });
        btExit.onClick.AddListener(() => { GameHandler.instance.askBeforeExitGame(); });

        //init achievement and leaderboard when gpgs is authenticated
        if (PlayCloudDataManager.isAuthenticated)
        {
            btAchievements.gameObject.SetActive(true);
            btLeaderboard.gameObject.SetActive(true);
            btAchievements.onClick.AddListener(() => { PlayCloudDataManager.instance.showAchivementsUI(); });
            btLeaderboard.onClick.AddListener(() => { PlayCloudDataManager.instance.showLeaderboardUI(); });
        }
        else
        {
            btAchievements.gameObject.SetActive(false);
            btLeaderboard.gameObject.SetActive(false);
        }

        //set highscore & resources
        MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, 0.25f, txtResources, false);

        if (StatsHandler.instance.Highscore != 0)
            MyUtilities.instance.textCountAnimation(StatsHandler.instance.Highscore, 0.25f, txtHighScore, false);
        else
            txtHighScore.text = "0";

        AchievementsHolder.checkAchivement(AchievementsHolder.achivementType.Be_a_SPACE_INVADER);
    }


    private IEnumerator initCoroutine()
    {
        while (!GameHandler.isInit) yield return null;

        init();
    }

    private void Update()
    {
        if (_42_isInit)
            return;

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _42_handler();
            }
        }

        if (Input.GetMouseButtonUp(0))
            _42_handler();
    }

    private void _42_handler()
    {
        _42_counter++;

        for (int i = 0; i < _42_counter; i++)
        {
            GameObject new_42_Hint = Instantiate(_42_hint_prefab);
            new_42_Hint.transform.position = MyUtilities.getRandomPositionOnDisplay();
            new_42_Hint.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 359));
        }        

        if (_42_counter == 42)
        {
            _42_isInit = true;
            AchievementsHolder.checkAchivement(AchievementsHolder.achivementType._42_);
            StartCoroutine(_42_Coroutine());
        }
    }

    private IEnumerator _42_Coroutine()
    {
        yield return null;

        while (txt_42_.text.Length < 6000)
        {            
            for (int i = 0; i < Random.Range(1, 5); i++)
            {
                txt_42_.text += " ";
            }

            txt_42_.text += 42;

            yield return new WaitForSeconds(_42_writeSpeed);
        }
    }

    


}
