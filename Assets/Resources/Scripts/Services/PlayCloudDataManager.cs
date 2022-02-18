// code reference: http://answers.unity3d.com/questions/894995/how-to-saveload-with-google-play-services.html		
// you need to import https://github.com/playgameservices/play-games-plugin-for-unity
using UnityEngine;
using System;
using System.Collections;
//gpg
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
//for encoding
using System.Text;
//for extra save ui
using UnityEngine.SocialPlatforms;
//for text, remove
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayCloudDataManager : MonoBehaviour
{
    private const string m_saveFileName = "BTSISavegame";

    public static Savegame currentSavegame = null;
    private ISavedGameMetadata savedGameMetaData;

    public bool initInProgress { get; private set; }
    public bool loginInProgress { get; private set; }
    public bool fileOpenInProgress { get; private set; }
    public bool loadingInProgress { get; private set; }
    public bool savingInProgress { get; private set; }
    public bool deletingInProgress { get; private set; }

    public static bool isAuthenticated
    {
        get
        {
            return Social.localUser.authenticated;
        }
    }

    public string generalLeaderboardID;


    public delegate void OnProgressFinished();
    private static OnProgressFinished onProgressFinished;


    public static PlayCloudDataManager instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void initAndLoad(OnProgressFinished finishedMethode)
    {
        initInProgress = true;

        if (!isAuthenticated || currentSavegame == null)
        {
            InitiatePlayGames();

            StartCoroutine(loadSavegameCoroutine(finishedMethode));
        }
        else
        {
            initInProgress = false;
            finishedMethode();
        }
    }

    private void InitiatePlayGames()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .EnableSavedGames()// enables saving game progress.
        .Build();

        PlayGamesPlatform.Initializeinstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;// recommended for debugging:
        PlayGamesPlatform.Activate();// Activate the Google Play Games platform
    }

    public void playGameSignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    private IEnumerator loadSavegameCoroutine(OnProgressFinished finishedMethode)
    {
        yield return null;

        GameHandler.instance.showHideLoadingProgressbar(true);

#if UNITY_EDITOR

#else
                Login();
        while (loginInProgress)
            yield return null;
#endif

        //load savegame
        SaveLoadSystem.instance.loadSaveGame();
        while (SaveLoadSystem.instance.isLoading)
            yield return null;

        initInProgress = false;

        finishedMethode();
    }


    public void Login()
    {
        loginInProgress = true;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Login successfully!");
                ((PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.TOP);
                loginInProgress = false;
            }
            else
            {
                Debug.Log("Login failed!");
                //PopupHandler.instance.showPopupWithTwoButton("Login to google play failed!", "Try again", delegate () { Login(); }, "Ok", delegate () { loginInProgress = false; });
                loginInProgress = false;
            }
        });
    }


    public int getDaysSinceLastPlayed()
    {
        int days = -1;
        ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        {
            // -1 means cached stats, 0 is succeess
            // see  CommonStatusCodes for all values.
            if (rc <= 0 && stats.HasDaysSinceLastPlayed())
            {
                days = stats.DaysSinceLastPlayed;
                //Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
            }
        });

        return days;
    }

    public void unlockAchievement(string achivementID)
    {
        if (!isAuthenticated)
            return;

        Social.ReportProgress(achivementID, 100.0f, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Achivement with ID: " + achivementID + " unlocked.");
            }
            else
            {
                Debug.Log("Achivement with ID: " + achivementID + " failed to unlock.");
            }
        });
    }

    public void showAchivementsUI()
    {
        if (isAuthenticated)
        {
            // show achievements UI
            Social.ShowAchievementsUI();
        }
    }

    public void postScoreToLeaderBoard(long score)
    {
        if (generalLeaderboardID == null)
            Debug.Log("There is no leaderboard ID.");

        // post score 12345 to leaderboard ID "Cfji293fjsie_QA")
        Social.ReportScore(score, generalLeaderboardID, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Score successfully uploaded.");
            }
            else
            {
                Debug.Log("Score upload failed.");
            }
        });
    }

    public void showLeaderboardUI(string specificLeaderboardID = null)
    {
        if (specificLeaderboardID == null)
        {
            // show leaderboard UI
            Social.ShowLeaderboardUI();
        }
        else
        {
            // show specific leaderboard UI
            PlayGamesPlatform.Instance.ShowLeaderboardUI(specificLeaderboardID);
        }
    }



    private void openFileForSaveAndLoad()
    {
        Debug.Log("Open File for saveing and loading.");

        fileOpenInProgress = true;

        ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            m_saveFileName, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            fileIsLoadedForSaveAndLoad);
    }

    private void fileIsLoadedForSaveAndLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("File opened for saving and loading.");
            savedGameMetaData = metaData;
        }
        else
        {
            Debug.Log("Cant open file for saving and loading: " + status);
        }
        fileOpenInProgress = false;
    }

    private void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData != null)
        {
            Debug.Log("Start convert bytes to Savegame.");
            currentSavegame = bytesSavegame(cloudData);

            if (currentSavegame != null)
            {
                Debug.Log("Savegame loaded successful.");
            }
            else
            {
                Debug.Log("Can't load Savegame!");
            }
        }
        else
        {
            Debug.LogWarning("No Data saved to the cloud.");
            currentSavegame = null;
        }
    }

    private byte[] savegameToBytes(Savegame savegame)
    {
        if (savegame == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, savegame);
            return ms.ToArray();
        }
    }

    private Savegame bytesSavegame(byte[] bytes)
    {
        Debug.Log("Savegame byte length: " + bytes.Length);
        if (bytes == null || bytes.Length == 0)
            return null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Savegame savegame = bf.Deserialize(ms) as Savegame;
                return savegame;
            }
        }
        catch
        {
            Debug.Log("Bytes to savegame failed!");
            return null;
        }
    }



    //Load----------------------------------------------------
    public void LoadFromCloud()
    {
        if (isAuthenticated && !loadingInProgress)
        {
            loadingInProgress = true;
            StartCoroutine(LoadFromCloudCoroutine());
        }
    }

    private IEnumerator LoadFromCloudCoroutine()
    {
        Debug.Log("Start loading savegame.");

        openFileForSaveAndLoad();
        while (fileOpenInProgress) yield return null;

        if (savedGameMetaData != null)
        {
            Debug.Log("Read binary data.");
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(savedGameMetaData, OnGameLoad);
        }
        else
        {
            Debug.LogWarning("No metadata for loading exist.");
            loadingInProgress = false;
        }
    }

    private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Process cloud data.");
            ProcessCloudData(bytes);
        }
        else
        {
            Debug.LogWarning("Error loading:" + status);
        }

        loadingInProgress = false;
    }



    //Save----------------------------------------------------
    public void SaveToCloud()
    {
        if (isAuthenticated)
        {
            StartCoroutine(SaveToCloudCoroutine());
        }
    }

    private IEnumerator SaveToCloudCoroutine()
    {
        /*while (savingInProgress)
            yield return null;*/

        savingInProgress = true;

        Debug.Log("Start saving savegame.");

        openFileForSaveAndLoad();
        while (fileOpenInProgress) yield return null;

        if (savedGameMetaData != null)
        {
            byte[] data = savegameToBytes(currentSavegame);

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            Debug.Log("Commit savegame update.");
            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(savedGameMetaData, updatedMetadata, data, OnGameSave);
        }
        else
        {
            Debug.LogWarning("No metadata for saving exist.");
            savingInProgress = false;
        }
    }


    private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Commit savegame update successful");
        }
        else
        {
            Debug.Log("Commit savegame update fail: " + status);
        }

        savingInProgress = false;
    }



    //Delete----------------------------------------------------
    public void deleteCloudData()
    {
        if (isAuthenticated && !deletingInProgress)
        {
            deletingInProgress = true;
            StartCoroutine(deleteCloudDataCoroutine());
        }
    }

    private IEnumerator deleteCloudDataCoroutine()
    {
        Debug.Log("Start delete savegame.");

        if (savedGameMetaData == null)
        {
            openFileForSaveAndLoad();

            while (fileOpenInProgress) yield return null;
        }

        if (savedGameMetaData != null)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(savedGameMetaData);
        }
        else
        {
            Debug.LogWarning("No metadata for saving exist.");
        }

        deletingInProgress = false;
    }

}
