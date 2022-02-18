using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class SaveLoadSystem : MonoBehaviour
{
    private const string local_saveFileName = "BTSISavegame.BTSIData";

    public bool isLoading { get; private set; }
    public bool isSaving { get; private set; }

    public static SaveLoadSystem instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public void loadSaveGame()
    {
        StartCoroutine(loadSavegameCoroutine());
    }

    private IEnumerator loadSavegameCoroutine()
    {
        while (isSaving || isLoading)
            yield return null;

        isLoading = true;

        //load savegame per gpgs or local
#if UNITY_EDITOR
        PlayCloudDataManager.currentSavegame = loadLocalData();
#else
                PlayCloudDataManager.instance.LoadFromCloud();
                while (PlayCloudDataManager.instance.loadingInProgress)
                {
                    yield return null;
                    Debug.Log("PlayCloudDataManager.instance.loadingInProgress ...");
                }
#endif


        if (PlayCloudDataManager.currentSavegame != null)//savegame exist
        {
            if (checkSavegameWipe(PlayCloudDataManager.currentSavegame.savegameVersion, GameHandler.staticLastSavegameWipeVersion))//when savegame version older then last wipe version => new savegame
            {
                newSaveGame();
                while (isSaving)
                    yield return null;
            }
            else //no savegame wipe
            {
                PlayCloudDataManager.currentSavegame.savegameVersion = Application.version;
            }
        }
        else//no savegame exists -> create new savegame
        {
            newSaveGame();

            while (isSaving)
                yield return null;
        }

        isLoading = false;
    }


    public void saveSavegame()
    {
        StartCoroutine(saveSavegameCoroutine());
    }

    private IEnumerator saveSavegameCoroutine()
    {
        while (isSaving || isLoading)
            yield return null;

        isSaving = true;

        Savegame currentsavegame = PlayCloudDataManager.currentSavegame;
        currentsavegame.formationData = FormationHandler.instance.save();
        currentsavegame.invadersData = saveAllInvaders();
        currentsavegame.statsData = StatsHandler.instance.save();

        //save game per gpgs or local
#if UNITY_EDITOR
        saveLocalData(PlayCloudDataManager.currentSavegame);
#else
                PlayCloudDataManager.instance.SaveToCloud();

                while (PlayCloudDataManager.instance.savingInProgress)
                    yield return null;
#endif


        isSaving = false;
    }

    private void newSaveGame()
    {
        PlayCloudDataManager.currentSavegame = createDefaultSaveGame();

        saveSavegame();
    }

    private Savegame createDefaultSaveGame()
    {
        Savegame tmpSavegame = new Savegame();

        tmpSavegame.savegameVersion = Application.version;

        tmpSavegame.soundIsOn = true;
        tmpSavegame.musicIsOn = true;

        tmpSavegame.tutorialData = new TutorialData();
        tmpSavegame.statsData = new StatsData();
        tmpSavegame.formationData = new FormationData();

        tmpSavegame.tutorialData.setDefault();
        tmpSavegame.statsData.setDefault();
        tmpSavegame.formationData.setDefault();

        List<GameObject> allDefaultInvaders = InvaderLoader.getAllInvaderPrefabs();
        InvadersData[] newDefaultInvadersData = new InvadersData[allDefaultInvaders.Count];
        for (int invaderIndex = 0; invaderIndex < newDefaultInvadersData.Length; invaderIndex++)
        {
            InvadersData newInvaderData = new InvadersData();
            Invader curDefaultInvaderScrp = allDefaultInvaders[invaderIndex].GetComponent<Invader>();

            newInvaderData.id = curDefaultInvaderScrp.id;
            newInvaderData.level = curDefaultInvaderScrp.level;
            //newInvaderData.hitpoints = curDefaultInvaderScrp.hitpointsDefaultLVL1;
            newInvaderData.unlocked = curDefaultInvaderScrp.unlocked;
            newInvaderData.unlockConditionsFullfilled = curDefaultInvaderScrp.getUnlockConditionsFullfilled();

            if (curDefaultInvaderScrp.weapons != null)
            {
                WeaponData[] defaultWeaponsDataList = new WeaponData[curDefaultInvaderScrp.weapons.Length];
                for (int weaponDataIndex = 0; weaponDataIndex < defaultWeaponsDataList.Length; weaponDataIndex++)
                {
                    WeaponData defaultWeaponData = new WeaponData();
                    ActiveWeapon currentActiveWeapon = curDefaultInvaderScrp.weapons[weaponDataIndex];

                    defaultWeaponData.invaderID = curDefaultInvaderScrp.id;
                    defaultWeaponData.weaponType = (int)currentActiveWeapon.weapon.GetComponent<Weapon>().weaponType;

                    /*defaultWeaponData.speed = currentActiveWeapon.speedDefaultLVL1;
                    defaultWeaponData.cooldown = currentActiveWeapon.cooldownDefaultLVL1;
                    defaultWeaponData.damage = currentActiveWeapon.damageDefaultLVL1;
                    defaultWeaponData.shieldTime = currentActiveWeapon.shieldTimeDefaultLVL1;
                    defaultWeaponData.shieldHitpoints = (int)System.Math.Round(curDefaultInvaderScrp.hitpointsDefaultLVL1 * currentActiveWeapon.shieldHitpointsMultiplier, 0);

                    defaultWeaponData.effectDuration = currentActiveWeapon.effectDurationDefaultLVL1;
                    defaultWeaponData.hitpointsPerSec = currentActiveWeapon.hitpointsPerSecDefaultLVL1;
                    defaultWeaponData.dronesQuantity = currentActiveWeapon.dronesQuantityDefaultLVL1;

                    defaultWeaponData.resourceExtraction = (uint)currentActiveWeapon.resourceExtractionDefaultLVL1;*/

                    defaultWeaponsDataList[weaponDataIndex] = defaultWeaponData;
                }

                newInvaderData.weaponData = defaultWeaponsDataList;
            }

            newDefaultInvadersData[invaderIndex] = newInvaderData;
        }
        tmpSavegame.invadersData = newDefaultInvadersData;

        return tmpSavegame;
    }


    private InvadersData[] saveAllInvaders()
    {
        InvadersData[] invadersData = new InvadersData[InvaderLoader.instance.getAllLoadedInvaders().Count];

        for (int i = 0; i < invadersData.Length; i++)
        {
            invadersData[i] = InvaderLoader.instance.getAllLoadedInvaders()[i].GetComponent<Invader>().save();
        }

        return invadersData;
    }

    public InvadersData loadInvader(int id)
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;

        if (savegame.invadersData != null)
        {
            for (int i = 0; i < savegame.invadersData.Length; i++)
                if (savegame.invadersData[i].id == id)
                    return savegame.invadersData[i];
        }

        return null;
    }

    public FormationData loadFormation()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;

        return savegame.formationData;
    }

    public StatsData loadStats()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;

        if (savegame.statsData != null)
        {
            return savegame.statsData;
        }

        return null;
    }

    public TutorialData getTutorialData()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;

        return savegame.tutorialData;
    }


    public bool loadSoundState()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;
        if (savegame != null)
            return savegame.soundIsOn;

        return false;
    }

    public bool loadMusicState()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;
        if (savegame != null)
            return savegame.musicIsOn;

        return false;
    }

    public bool loadShowFPS()
    {
        Savegame savegame = PlayCloudDataManager.currentSavegame;
        if (savegame != null)
            return savegame.showFPS;

        return false;
    }



    /*
    * if savegame version older then last wipe version return true
    */
    private bool checkSavegameWipe(string savegameVersion, string lastWipeVersion)
    {
        string versionsCheckDebug = "";
        versionsCheckDebug += "savegame version: " + savegameVersion;
        versionsCheckDebug += "\nlast wipe version: " + lastWipeVersion;

        if (savegameVersion == null || lastWipeVersion == null)
        {
            Debug.Log(versionsCheckDebug);
            return false;
        }

        string[] savegameVersionParts = savegameVersion.Split('.');
        string[] lastWipeVersionParts = lastWipeVersion.Split('.');

        int tmpsavegameVersionPart;
        int tmplastWipeVersionPart;
        for (int i = 0; i < savegameVersionParts.Length; i++)
        {
            tmpsavegameVersionPart = -1;
            tmplastWipeVersionPart = -1;
            int.TryParse(savegameVersionParts[i], out tmpsavegameVersionPart);
            int.TryParse(lastWipeVersionParts[i], out tmplastWipeVersionPart);

            if (tmpsavegameVersionPart != -1 && tmplastWipeVersionPart != -1)
            {
                if (tmpsavegameVersionPart < tmplastWipeVersionPart)
                {
                    versionsCheckDebug += "\n\t" + tmpsavegameVersionPart + " < " + tmplastWipeVersionPart + " = true";
                    Debug.Log(versionsCheckDebug);
                    return true;
                }
                else
                {
                    versionsCheckDebug += "\n\t" + tmpsavegameVersionPart + " < " + tmplastWipeVersionPart + " = false";
                }
            }
        }

        return false;
    }

    public void removeSavegame()
    {
        StartCoroutine(removeSavegameCoroutine());
    }

    private IEnumerator removeSavegameCoroutine()
    {
        yield return null;

        GameHandler.instance.showHideLoadingProgressbar(true);

        removeLocalSavegame();


#if UNITY_EDITOR
        removeLocalSavegame();
#else
              PlayCloudDataManager.instance.deleteCloudData();
            while (PlayCloudDataManager.instance.deletingInProgress)
                yield return null;  
#endif


        PlayCloudDataManager.currentSavegame = null;

        GameHandler.instance.showMainMenue(true);
    }

    public bool removeLocalSavegame()
    {
        string path = Application.persistentDataPath + "/ " + local_saveFileName;
        if (File.Exists(path))
        {
            File.Delete(path);

            return true;
        }

        return false;
    }

    private void saveLocalData(object savegameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ " + local_saveFileName;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, savegameData);
        stream.Close();
    }

    private Savegame loadLocalData()
    {
        string path = Application.persistentDataPath + "/ " + local_saveFileName;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            object objectData = formatter.Deserialize(stream);
            stream.Close();

            return objectData as Savegame;
        }
        else
        {
            return null;
        }
    }


}
