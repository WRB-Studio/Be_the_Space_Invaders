using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchivementHolder", menuName = "ScriptableObjects/AchivementHolder", order = 1)]
public class AchievementsHolder : ScriptableObject
{
    public List<AchievementObject> achivements = new List<AchievementObject>();

    public enum achivementType
    {
        Be_a_SPACE_INVADER,
        Small_ships_Captain,
        Defense_Officer,
        Support_Officer,
        Large_ships_Admiral,
        Fleet_Commander,
        _50_Kills,
        _100_Kills,
        _500_Kills,
        _1K_Kills,
        _5K_Kills,
        _10K_Kills,
        _25K_Kills,
        _50K_Kills,
        _42_
    }


    public static void checkAchivement(achivementType achivement)
    {
        switch (achivement)
        {
            case achivementType.Be_a_SPACE_INVADER:
                check_Be_a_SPACE_INVADER();
                break;
            case achivementType._42_:
                _42_();
                break;
            default:
                break;
        }
    }

    public static void checkShipBuyAchivements()
    {
        check_Small_ships_Captain();
        check_Defense_Officer();
        check_Support_Officer();
        check_Large_ships_Admiral();
        check_Fleet_Commander();
    }

    public static void checkKillsAchivement()
    {
        check_50_Kills();
        check_100_Kills();
        check_500_Kills();
        check_1K_Kills();
        check_5K_Kills();
        check_10K_Kills();
        check_25K_Kills();
        check_50K_Kills();
    }


    public static void check_Be_a_SPACE_INVADER()
    {
        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQAQ");
    }

    public static void _42_()
    {
        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQEA");
    }


    public static void check_Small_ships_Captain()
    {
        List<GameObject> loadedShips = InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Attacker);
        for (int i = 0; i < loadedShips.Count; i++)
        {
            if (!loadedShips[i].GetComponent<Invader>().unlocked)
                return;
        }

        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQAw");
    }

    public static void check_Defense_Officer()
    {
        List<GameObject> loadedShips = InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Defender);
        for (int i = 0; i < loadedShips.Count; i++)
        {
            if (!loadedShips[i].GetComponent<Invader>().unlocked)
                return;
        }

        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQBA");
    }

    public static void check_Support_Officer()
    {
        List<GameObject> loadedShips = InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Supporter);
        for (int i = 0; i < loadedShips.Count; i++)
        {
            if (!loadedShips[i].GetComponent<Invader>().unlocked)
                return;
        }

        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQBQ");
    }

    public static void check_Large_ships_Admiral()
    {
        List<GameObject> loadedShips = InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Boss);
        for (int i = 0; i < loadedShips.Count; i++)
        {
            if (!loadedShips[i].GetComponent<Invader>().unlocked)
                return;
        }

        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQBg");
    }

    public static void check_Fleet_Commander()
    {
        List<GameObject> loadedShips = InvaderLoader.instance.getAllLoadedInvaders();
        for (int i = 0; i < loadedShips.Count; i++)
        {
            if (!loadedShips[i].GetComponent<Invader>().unlocked)
                return;
        }

        PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQBw");
    }


    public static void check_50_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 50)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQDg");
    }

    public static void check_100_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 100)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQCA");
    }

    public static void check_500_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 500)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQDw");
    }

    public static void check_1K_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 1000)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQCQ");
    }

    public static void check_5K_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 5000)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQCg");
    }

    public static void check_10K_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 10000)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQCw");
    }

    public static void check_25K_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 25000)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQDA");
    }

    public static void check_50K_Kills()
    {
        if (StatsHandler.instance.totalTerranKills >= 50000)
            PlayCloudDataManager.instance.unlockAchievement("CgkIwIzq5rwaEAIQDQ");
    }
}
