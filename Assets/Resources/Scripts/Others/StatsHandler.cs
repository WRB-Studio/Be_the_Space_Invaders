using System.Collections.Generic;
using UnityEngine;

public class StatsHandler : MonoBehaviour
{
    public static uint startResources = 0;
    private static float resourceMultiplicator = 1.3f;
    private static float scoreMultiplicator = 1.26f;


    /*---Total game stats---*/
    private uint totalPlayedTime;
    private uint resources;
    private uint totalCollectedResources;
    private uint highscore;
    private int totalLostInvaders;
    private int totalKilledTerranFighters;
    private int totalKilledTerranFrigates;
    private int totalKilledTerranBattleships;
    private int totalKilledStations;
    private int totalAsteroidKills;
    private int totalShootKills;
    private int shotsFired;
    private int shotHits;


    public uint TotalPlayedTime { get { return totalPlayedTime; } }

    public uint Resources { get { return resources; } }
    public uint TotalCollectedResources { get { return totalCollectedResources; } }

    public uint Highscore { get { return highscore; } }

    public int TotalLostInvaders { get { return totalLostInvaders; } }

    public int TotalKilledTerranFighters { get { return totalKilledTerranFighters; } }
    public int TotalKilledTerranFrigates { get { return totalKilledTerranFrigates; } }
    public int TotalKilledTerranBattleships { get { return totalKilledTerranBattleships; } }
    public int TotalKilledStations { get { return totalKilledStations; } }
    public int totalTerranKills { get { return TotalKilledTerranFighters + totalKilledTerranFrigates + totalKilledTerranBattleships + totalKilledStations; } }

    public int TotalAsteroidKills { get { return totalAsteroidKills; } }
    public int TotalShootKills { get { return totalShootKills; } }

    public int ShotsFired { get { return shotsFired; } }
    public int ShotHits { get { return shotHits; } }


    /*---specific ship stats---*/
    public int hunterUses;
    public int bomberUses;
    public int corvetteUses;
    public int defenderShieldUses;
    public int starWallShieldUses;
    public int engineerRepairUses;
    public int scrapCollectorUses;
    public int ai_supporterUses;
    public int destroyerUses;
    public int cruiserUses;
    public int behemothUses;

    public int hunterKills;
    public int bomberKills;
    public int corvetteKills;
    public int destroyerKills;
    public int cruiserKills;
    public int behemothKills;

    public int scrapCrusherDefences;
    public int defenderShieldDefences;
    public int starWallShieldDefences;

    public uint scrapCrusherResGain;
    public uint scrapCollectorResGain;
    public uint manualResGain;

    public uint scrapCrusherCollectedResPieces;
    public uint scrapCollectorCollectedResPieces;
    public uint manualCollectedResPieces;

    public int highestScoreMultiplier;
    public uint highestMultiplierScore;


    /*---Current game stats---*/
    private uint currentPlayedTime;
    private uint currentScore;
    private uint backupresourcesForCurrentGame;
    private uint currentCollectedResources;
    private int currentKilledTerrans;
    private int currentKilledStations;
    private int currentKilledAsteroids;


    public uint CurrentPlayedTime { get { return currentPlayedTime; } set { currentPlayedTime += value; addTotalPlayedTime(value); } }
    public uint CurrentScore { get { return currentScore; } }
    public uint BackupResourcesForCurrentGame { get { return backupresourcesForCurrentGame; } }
    public uint CurrentCollectedResources { get { return currentCollectedResources; } }
    public int CurrentKilledTerrans { get { return currentKilledTerrans; } set { currentKilledTerrans += value; } }
    public int CurrentKilledStations { get { return currentKilledStations; } set { currentKilledStations += value; addTotalKilledStations(value); } }
    public int CurrentKilledAsteroids { get { return currentKilledAsteroids; } set { currentKilledAsteroids += value; addTotalAsteroidKills(value); } }
    public float CurrentResourceMultiplicator
    {
        get
        {
            float calculatedMultiplicator;

            if (currentKilledStations <= 0)
                calculatedMultiplicator = resourceMultiplicator;
            else
                calculatedMultiplicator = resourceMultiplicator + (0.5f * currentKilledStations);

            return calculatedMultiplicator;
        }
    }


    public static StatsHandler instance;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void init()
    {
        load();
    }

    public void resetCurrentGameStats()
    {
        currentPlayedTime = 0;
        currentScore = 0;
        backupresourcesForCurrentGame = 0;
        currentCollectedResources = 0;
        currentKilledTerrans = 0;
        currentKilledStations = 0;
        currentKilledAsteroids = 0;
    }

    public void addTotalPlayedTime(uint addVal)
    {
        totalPlayedTime += addVal;
    }

    public void saveBackupResourcesForCurrentGame()
    {
        backupresourcesForCurrentGame = resources;
    }
    public void addResources(uint addVal)
    {
        resources += addVal;
        addTotalCollectedResources(addVal);
    }

    public void subResources(uint subVal)
    {
        resources -= subVal;
    }

    public void addTotalCollectedResources(uint addVal)
    {
        totalCollectedResources += addVal;
    }

    public void checkNewHighscore(uint newVal)
    {
        if (newVal > highscore)
        {
            highscore = newVal;
        }
    }

    public void addTotalLostInvaders(int addVal)
    {
        totalLostInvaders += addVal;
    }

    public void addTotalKilledTerranFighters(int addVal)
    {
        totalKilledTerranFighters += addVal;
        AchievementsHolder.checkKillsAchivement();
    }

    public void addTotalKilledTerranFrigates(int addVal)
    {
        totalKilledTerranFrigates += addVal;
        AchievementsHolder.checkKillsAchivement();
    }

    public void addTotalKilledTerranBattleships(int addVal)
    {
        totalKilledTerranBattleships += addVal;
        AchievementsHolder.checkKillsAchivement();
    }

    public void addTotalKilledStations(int addVal)
    {
        totalKilledStations += addVal;
        AchievementsHolder.checkKillsAchivement();
    }

    public void addTotalAsteroidKills(int addVal)
    {
        totalAsteroidKills += addVal;
    }

    public void addTotalShootKills(int addVal)
    {
        totalShootKills += addVal;
    }

    public void addShotsFired(int addVal, string shipname)
    {
        if (shipname != null)
            addUsesByInvaderType(shipname);

        shotsFired += addVal;
    }

    public void addShotHits(int addVal)
    {
        shotHits += addVal;
    }


    public uint addCurrentScoreWithFeedback(uint newScore, string fromObject, Vector3 feedbackPosition)
    {
        IngameHandler.instance.showPointsFeedback(newScore, feedbackPosition, new Color(0.03424335f, 1f, 0f, 1f));

        return addCurrentScore(newScore, fromObject);
    }

    public uint addCurrentScore(uint newScore, string fromObject)
    {
        int tmpCurrentKilledStations = currentKilledStations;
        if (tmpCurrentKilledStations < 1)
            tmpCurrentKilledStations = 1;
        uint recalculatedScore = (uint)Mathf.RoundToInt(newScore * scoreMultiplicator * tmpCurrentKilledStations);

        IngameHandler.instance.refreshTxtScore(currentScore, recalculatedScore);
        currentScore += recalculatedScore;

        return recalculatedScore;
    }


    public uint addCurrentCollectedResources(uint collectedResources, bool refreshTxtRes = true, bool showFeedback = false, Vector3 feedbackPosition = new Vector3())
    {
        uint extraResourcesPerStage = 0;
        for (int i = 1; i < StageHandler.instance.currentStage; i++)
        {
            if (MyUtilities.isMultipleOf(i, 3))
                extraResourcesPerStage++;
        }

        uint recalculateCollectedResources = (uint)Mathf.RoundToInt((collectedResources + extraResourcesPerStage) * CurrentResourceMultiplicator);

        if (showFeedback)
            IngameHandler.instance.showPointsFeedback(recalculateCollectedResources, feedbackPosition, new Color(0.8584906f, 0.594332f, 0f, 1));

        if (refreshTxtRes)
            IngameHandler.instance.refreshTxtResources(currentCollectedResources, recalculateCollectedResources);

        currentCollectedResources += recalculateCollectedResources;
        addTotalCollectedResources(recalculateCollectedResources);
        addResources(recalculateCollectedResources);

        return recalculateCollectedResources;
    }


    public void addUsesByInvaderType(string shipname)
    {
        switch (shipname)
        {
            case "Hunter":
                hunterUses++;
                break;
            case "Bomber":
                bomberUses++;
                break;
            case "Corvette":
                corvetteUses++;
                break;

            case "Scrap Crusher":
                break;
            case "Defender":
                defenderShieldUses++;
                break;
            case "Star Wall":
                starWallShieldUses++;
                break;

            case "Engineer":
                engineerRepairUses++;
                break;
            case "Scrap Collector":
                scrapCollectorUses++;
                break;
            case "AI-Spporter":
                ai_supporterUses++;
                break;

            case "Destroyer":
                destroyerUses++;
                break;
            case "Cruiser":
                cruiserUses++;
                break;
            case "Behemoth":
                behemothUses++;
                break;

            default:
                break;
        }
    }

    public void addKillByInvaderType(string shipname)
    {
        switch (shipname)
        {
            case "Hunter":
                hunterKills++;
                break;
            case "Bomber":
                bomberKills++;
                break;
            case "Corvette":
                corvetteKills++;
                break;

            case "Destroyer":
                destroyerKills++;
                break;
            case "Cruiser":
                cruiserKills++;
                break;
            case "Behemoth":
                behemothKills++;
                break;

            default:
                break;
        }
    }

    public void addDefencesByInvaderType(string shipname)
    {
        if (shipname.Contains("Scrap Crusher"))
            scrapCrusherDefences++;
        else if (shipname.Contains("Defender"))
            defenderShieldDefences++;
        else if (shipname.Contains("Star Wall"))
            starWallShieldDefences++;
    }

    public void addResGainByInvaderType(string shipname, uint gainedRes)
    {
        switch (shipname)
        {
            case "Scrap Crusher":
                scrapCrusherResGain += gainedRes;
                scrapCrusherCollectedResPieces++;
                break;

            case "Scrap Collector":
                scrapCollectorResGain += gainedRes;
                scrapCollectorCollectedResPieces++;
                break;

            default:
                break;
        }
    }

    public void addManualResGain(uint gainedRes)
    {
        manualResGain += gainedRes;
        manualCollectedResPieces++;
    }

    public void checkForHighestScoreMultiplier(int multiplierVal, uint multiplierScoreVal)
    {
        if (highestScoreMultiplier < multiplierVal)
            highestScoreMultiplier = multiplierVal;

        if (highestMultiplierScore < multiplierScoreVal)
            highestMultiplierScore = multiplierScoreVal;
    }



    /*------------Save and Load methods---------------*/

    public StatsData save()
    {
        StatsData data = new StatsData();

        data.totalPlayedTime = totalPlayedTime;

        data.resources = resources;
        data.totalCollectedResources = totalCollectedResources;

        data.highscore = highscore;

        data.totalLostInvaders = totalLostInvaders;

        data.totalKilledTerranFighters = totalKilledTerranFighters;
        data.totalKilledTerranFrigates = totalKilledTerranFrigates;
        data.totalKilledTerranBattleships = totalKilledTerranBattleships;

        data.totalKilledStations = totalKilledStations;

        data.totalAsteroidKills = totalAsteroidKills;
        data.totalShootKills = totalShootKills;

        data.shotsFired = shotsFired;
        data.shotsHit = shotHits;


        data.hunterUses = hunterUses;
        data.bomberUses = bomberUses;
        data.corvetteUses = corvetteUses;
        data.defenderShieldUses = defenderShieldUses;
        data.starWallShieldUses = starWallShieldUses;
        data.engineerRepairUses = engineerRepairUses;
        data.scrapCollectorUses = scrapCollectorUses;
        data.ai_supporterUses = ai_supporterUses;
        data.destroyerUses = destroyerUses;
        data.cruiserUses = cruiserUses;
        data.behemothUses = behemothUses;

        data.hunterKills = hunterKills;
        data.bomberKills = bomberKills;
        data.corvetteKills = corvetteKills;
        data.destroyerKills = destroyerKills;
        data.cruiserKills = cruiserKills;
        data.behemothKills = behemothKills;

        data.scrapCrusherDefences = scrapCrusherDefences;
        data.defenderShieldDefences = defenderShieldDefences;
        data.starWallShieldDefences = starWallShieldDefences;

        data.scrapCrusherResGain = scrapCrusherResGain;
        data.scrapCollectorResGain = scrapCollectorResGain;
        data.manualResGain = manualResGain;

        data.scrapCrusherCollectedResPieces = scrapCrusherCollectedResPieces;
        data.scrapCollectorCollectedResPieces = scrapCollectorCollectedResPieces;
        data.manualCollectedResPieces = manualCollectedResPieces;

        data.highestScoreMultiplier = highestScoreMultiplier;
        data.highestMultiplierScore = highestMultiplierScore;

        return data;
    }

    public void load()
    {
        StatsData data = SaveLoadSystem.instance.loadStats();

        totalPlayedTime = data.totalPlayedTime;

        resources = data.resources;
        totalCollectedResources = data.totalCollectedResources;

        highscore = data.highscore;

        totalLostInvaders = data.totalLostInvaders;

        totalKilledTerranFighters = data.totalKilledTerranFighters;
        totalKilledTerranFrigates = data.totalKilledTerranFrigates;
        totalKilledTerranBattleships = data.totalKilledTerranBattleships;

        totalKilledStations = data.totalKilledStations;

        totalAsteroidKills = data.totalAsteroidKills;
        totalShootKills = data.totalShootKills;

        shotsFired = data.shotsFired;
        shotHits = data.shotsHit;


        hunterUses = data.hunterUses;
        bomberUses = data.bomberUses;
        corvetteUses = data.corvetteUses;
        defenderShieldUses = data.defenderShieldUses;
        starWallShieldUses = data.starWallShieldUses;
        engineerRepairUses = data.engineerRepairUses;
        scrapCollectorUses = data.scrapCollectorUses;
        ai_supporterUses = data.ai_supporterUses;
        destroyerUses = data.destroyerUses;
        cruiserUses = data.cruiserUses;
        behemothUses = data.behemothUses;

        hunterKills = data.hunterKills;
        bomberKills = data.bomberKills;
        corvetteKills = data.corvetteKills;
        destroyerKills = data.destroyerKills;
        cruiserKills = data.cruiserKills;
        behemothKills = data.behemothKills;

        scrapCrusherDefences = data.scrapCrusherDefences;
        defenderShieldDefences = data.defenderShieldDefences;
        starWallShieldDefences = data.starWallShieldDefences;

        scrapCrusherResGain = data.scrapCrusherResGain;
        scrapCollectorResGain = data.scrapCollectorResGain;
        manualResGain = data.manualResGain;

        scrapCrusherCollectedResPieces = data.scrapCrusherCollectedResPieces;
        scrapCollectorCollectedResPieces = data.scrapCollectorCollectedResPieces;
        manualCollectedResPieces = data.manualCollectedResPieces;

        highestScoreMultiplier = data.highestScoreMultiplier;
        highestMultiplierScore = data.highestMultiplierScore;

    }

}
