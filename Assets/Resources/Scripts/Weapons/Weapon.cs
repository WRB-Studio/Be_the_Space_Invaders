using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{    
    public enum WeaponType
    {
        None, Laser, Missle_launcher, Bomb, Scattering_laser,
        Absorbing_shield, Deflector_shield,
        Repair_drone, Collector_drone, Targeting_system
    }

    public WeaponType weaponType = WeaponType.None;

    [Header("Weapon settings")]
    public float maxSpeed;
    public float minSpeed;
    public float minCooldown;
    public GameObject shootPrefabInvader;
    public GameObject shootPrefabTerrans;

    [Header("Shield stats")]
    public float minShieldTime;
    public float maxShieldTime;
    public int minShieldHitpoints;

    [Header("Supporter stats")]
    public float maxEffectDuration;
    public float minEffectDuration;
    public int minHitpointsPerSec;
    public int maxDronesQuantity;

}