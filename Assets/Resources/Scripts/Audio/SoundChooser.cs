using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChooser : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menu_music;
    public AudioClip ingame_music;
    public AudioClip gameOver_music;

    [Header("Shot init sounds")]
    public AudioClip laserShot_Invader;
    public AudioClip laserShot_Terran;
    public AudioClip bombShot_Invader;
    public AudioClip bombShot_Terran;
    public AudioClip missileShot_Invader;
    public AudioClip missileShot_Terran;

    [Header("Shot impact sounds")]
    public AudioClip laserHit;
    public AudioClip bombHit;
    public AudioClip missileHit;

    [Header("Explosion sound init sounds")]
    public AudioClip explosion_Invader;
    public AudioClip explosion_Terran;
    public AudioClip explosion_Asteroid;

    [Header("Shield sounds")]
    public AudioClip activateShield;
    public AudioClip deactivateShield;
    public AudioClip shieldImpact;

    [Header("Support system sound")]
    public AudioClip activateSupportSystem;

    [Header("Others")]
    public AudioClip collectResources;
    public AudioClip hyperdriveInvaders;
    public AudioClip hyperdriveTerrans;
    public AudioClip fillStationHPBar;

    [Header("UI sounds")]
    public AudioClip gameOver;
    public AudioClip scoreCount;
    public AudioClip openPopup;
    public AudioClip sceneSwitch;

    [Header("Formation menu sounds")]
    public AudioClip dragInvader;
    public AudioClip successfullInvaderDrop;
    public AudioClip failedInvaderDrop;
    public AudioClip buyInvader;
    public AudioClip upgradeInvader;
    public AudioClip cantUpgradeOrBuy;
    public AudioClip buyFormationSlot;
    public AudioClip removeFormation;


    public static SoundChooser instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

}
