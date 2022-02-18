using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Shield infos")]
    private ActiveWeapon weaponData;
    public Weapon.WeaponType weaponType = Weapon.WeaponType.None;
    public float shieldTime;
    public float currentShieldTime;
    public int hitpoints;
    public float shieldSizeX;
    public bool isFromInvader { get; private set; }

    [Header("Color settings")]
    public Color invaderShieldColor;
    public Color terranShieldColor;

    [Header("Gain settings")]
    public uint score;

    private bool isInit = true;



    public static GameObject instantiateShield(ActiveWeapon weaponDataVal)
    {
        GameObject newShield = null;
        if (weaponDataVal.weaponType == Weapon.WeaponType.Absorbing_shield)
            newShield = Instantiate(Resources.Load<GameObject>("Prefabs/Shields/AbsorbingShield"));
        else if (weaponDataVal.weaponType == Weapon.WeaponType.Deflector_shield)
            newShield = Instantiate(Resources.Load<GameObject>("Prefabs/Shields/DeflectorShield"));

        newShield.GetComponent<Shield>().init(weaponDataVal);

        return newShield;
    }

    public void init(ActiveWeapon weaponDataVal)
    {
        weaponData = weaponDataVal;

        isFromInvader = weaponData.isInvaderWeapon;

        weaponType = weaponData.weaponType;
        shieldTime = weaponData.shieldTime;
        currentShieldTime = shieldTime;
        hitpoints = weaponData.shieldHitpoints;

        if (isFromInvader)
        {
            GetComponent<SpriteRenderer>().color = invaderShieldColor;
            transform.Rotate(0, 0, 180);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = terranShieldColor;
        }

        StaticAudioHandler.playSound(SoundChooser.instance.activateShield);

        isInit = true;
    }

    public void updateCall()
    {
        if (!isInit)
            return;

        currentShieldTime -= Time.fixedDeltaTime;

        if (currentShieldTime <= 0)
            destroyShield();
    }


    private void deflectorLaserHit(Shoot shoot)
    {
        shoot.gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(enableShootColliderAfterHitCoroutine(shoot.gameObject));
        shoot.onDeflectorShieldCollision();
    }

    private IEnumerator enableShootColliderAfterHitCoroutine(GameObject hittedShoot)
    {
        yield return new WaitForSeconds(0.3f);
        if (hittedShoot != null)
            hittedShoot.GetComponent<Collider2D>().enabled = true;
    }

    public void hit(int damageVal, Vector3 collisionPoint)
    {
        hitpoints -= damageVal;
        if (hitpoints <= 0)
        {
            hitpoints = 0;

            if (!isFromInvader)
                StatsHandler.instance.addCurrentScoreWithFeedback(score, "Terran shield hit", collisionPoint);

            destroyShield();
        }
    }

    public void destroyShield(Collision2D collision = null)
    {
        StartCoroutine(destroyShieldDelayCoroutine(collision));
    }

    private IEnumerator destroyShieldDelayCoroutine(Collision2D collision = null)
    {
        yield return new WaitForSeconds(0.05f);

        StaticAudioHandler.playSound(SoundChooser.instance.deactivateShield);

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isInit)
            return;

        GameObject colliderGO = col.collider.gameObject;
        Vector3 colliderPosition = col.contacts[0].point;

        if (isFromInvader)//if shield is from invader
        {
            if (colliderGO.tag == "Shoot" && !colliderGO.GetComponent<Shoot>().isFromInvader)
            {
                StaticAudioHandler.playSound(SoundChooser.instance.shieldImpact);

                Shoot shoot = colliderGO.GetComponent<Shoot>();

                if (weaponType == Weapon.WeaponType.Absorbing_shield)
                {
                    shoot.destroyShoot();
                }
                else
                {
                    if (shoot.weaponType == Weapon.WeaponType.Laser)
                    {
                        deflectorLaserHit(shoot);
                    }
                    else
                    {
                        shoot.destroyShoot();
                    }
                }

                StatsHandler.instance.addDefencesByInvaderType(weaponData.invaderScrp.name);

                hit(shoot.damage, colliderPosition);
                return;
            }
            else if (colliderGO.tag == "Asteroid")
            {
                hit(colliderGO.GetComponent<Asteroid>().damage, colliderPosition);
                colliderGO.GetComponent<Asteroid>().destroyAsteroid();
            }
        }

    }

}
