using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenuHandler : MonoBehaviour
{
    public Button btMainMenu;
    public Text txtResources;

    public Text txtHighscore;
    public Text txtBestMultiplicator;
    public Text txtPlayedTime;
    public Text txtCollectedScrap;
    public Text txtLostInvaders;
    public Text txtTerranFightersKills;
    public Text txtTerranFrigates;
    public Text txtTerranBattleships;
    public Text txtTotalTerranKills;
    public Text txtStationKills;
    public Text txtDestroyedAsteroids;
    public Text txtDefendedShots;
    public Text txtShots;
    public Text txtHits;
    public Text txtShotHitRatio;



    private void Start()
    {
        btMainMenu.onClick.AddListener(() => { GameHandler.instance.showMainMenue(); });

        MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, 0.25f, txtResources, false);

        TimeSpan timeSpan = TimeSpan.FromSeconds(StatsHandler.instance.TotalPlayedTime);
        txtPlayedTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        MyUtilities.instance.textCountAnimation(StatsHandler.instance.Highscore, 0.25f, txtHighscore, false);
        txtBestMultiplicator.text = "x" + StatsHandler.instance.highestScoreMultiplier + " (" + StatsHandler.instance.highestMultiplierScore + ")";

        MyUtilities.instance.textCountAnimation(StatsHandler.instance.TotalCollectedResources, 0.25f, txtCollectedScrap, false);

        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalLostInvaders, 0.25f, txtLostInvaders, false);

        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalKilledTerranFighters, 0.25f, txtTerranFightersKills, false);
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalKilledTerranFrigates, 0.25f, txtTerranFrigates, false);
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalKilledTerranBattleships, 0.25f, txtTerranBattleships, false);
        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalKilledStations, 0.25f, txtStationKills, false);
        int totalKilledTerrans = StatsHandler.instance.TotalKilledTerranFighters + StatsHandler.instance.TotalKilledTerranFrigates + StatsHandler.instance.TotalKilledTerranBattleships + StatsHandler.instance.TotalKilledStations;
        MyUtilities.instance.textCountAnimation((uint)totalKilledTerrans, 0.25f, txtTotalTerranKills, false);

        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalAsteroidKills, 0.25f, txtDestroyedAsteroids, false);

        MyUtilities.instance.textCountAnimation((uint)StatsHandler.instance.TotalShootKills, 0.25f, txtDefendedShots, false);

        float hitRate = 0;
        if (StatsHandler.instance.ShotHits == 0 && StatsHandler.instance.ShotsFired == 0)
            hitRate = 0;
        else if (StatsHandler.instance.ShotHits == StatsHandler.instance.ShotsFired)
            hitRate = 100;
        else
            hitRate = StatsHandler.instance.ShotHits / ((float)StatsHandler.instance.ShotsFired / 100);
        hitRate = (float)Math.Round(hitRate, 2);

        txtShots.text = StatsHandler.instance.ShotsFired.ToString();
        txtHits.text = StatsHandler.instance.ShotHits.ToString();
        txtShotHitRatio.text = hitRate + " %";
    }

}
