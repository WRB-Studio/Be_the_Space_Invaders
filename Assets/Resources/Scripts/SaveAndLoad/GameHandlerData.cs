[System.Serializable]
public class StatsData
{
    public uint totalPlayedTime;

    public uint resources;
    public uint totalCollectedResources;

    public uint highscore;

    public int totalLostInvaders;

    public int totalKilledTerranFighters;
    public int totalKilledTerranFrigates;
    public int totalKilledTerranBattleships;

    public int totalKilledStations;

    public int totalAsteroidKills;
    public int totalShootKills;

    public int shotsFired;
    public int shotsHit;


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

    public void setDefault()
    {
        resources = StatsHandler.startResources;
        totalCollectedResources = 0;

        totalPlayedTime = 0;

        highscore = 0;

        totalLostInvaders = 0;

        totalKilledTerranFighters = 0;
        totalKilledTerranFrigates = 0;
        totalKilledTerranBattleships = 0;

        totalKilledStations = 0;

        totalAsteroidKills = 0;
        totalShootKills = 0;

        shotsFired = 0;
        shotsHit = 0;


        hunterUses = 0;
        bomberUses = 0;
        corvetteUses = 0;
        defenderShieldUses = 0;
        starWallShieldUses = 0;
        engineerRepairUses = 0;
        scrapCollectorUses = 0;
        ai_supporterUses = 0;
        destroyerUses = 0;
        cruiserUses = 0;
        behemothUses = 0;

        hunterKills = 0;
        bomberKills = 0;
        corvetteKills = 0;
        destroyerKills = 0;
        cruiserKills = 0;
        behemothKills = 0;

        scrapCrusherDefences = 0;
        defenderShieldDefences = 0;
        starWallShieldDefences = 0;

        scrapCrusherResGain = 0;
        scrapCollectorResGain = 0;
        manualResGain = 0;

        scrapCrusherCollectedResPieces = 0;
        scrapCollectorCollectedResPieces = 0;
        manualCollectedResPieces = 0;

        highestScoreMultiplier = 0;
        highestMultiplierScore = 0;
    }

}
