using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerranStation : MonoBehaviour
{
    [Header("Station infos")]
    public int id;
    public int level;
    public int hitpoints;
    private int currentHitpoints;

    [Header("Movement settings")]
    private float survivedTime = 0;

    [Header("Weapon settings")]
    public ActiveWeapon[] weapons;
    private bool isShooting = false;

    [Header("Gain settings")]
    public int score;
    public int minScrapAmount = 3;
    public int maxScrapAmount = 6;

    [Header("Animation settings")]
    public GameObject explosionEffectPrefab;

    private bool isInit = false;



    #region general methods

    public void init()
    {
        isInit = true;
    }

    public bool getIsInit()
    {
        return isInit;
    }

    public void calculateStationStats(int currentStage)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].init();

        levelUp(currentStage);
    }

    public void levelUp(int currentStage)
    {
        level = currentStage;

        hitpoints = hitpoints * level;
        currentHitpoints = hitpoints;

        for (int i = 0; i < weapons.Length; i++)
        {
            for (int levelCount = 1; levelCount < level; levelCount++)
            {
                weapons[i].upgrade(level);
            }
        }
    }

    public void updateCall()
    {
        if (!isInit || weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        survivedTime += Time.fixedDeltaTime;

        weaponUpdate();
    }

    #endregion


    #region Movement methods
        
    #endregion


    #region Weapon methods  

    public ActiveWeapon[] getWeaponsOfType(Weapon.WeaponType weapontype)
    {
        List<ActiveWeapon> weaponsOfType = new List<ActiveWeapon>();
        for (int weaponIndex = 0; weaponIndex < weapons.Length; weaponIndex++)
        {
            if (weapons[weaponIndex].weapon.GetComponent<Weapon>().weaponType == weapontype)
                weaponsOfType.Add(weapons[weaponIndex]);
        }

        return weaponsOfType.ToArray();
    }

    private void weaponUpdate()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].updateCall();

            if (isShooting && !weapons[i].cooldownFinished())//check ship is shooting and weapon cooldown is finished
                continue;

            //if weapon is no missle launcher
            if (weapons[i].weapon.GetComponent<Weapon>().weaponType != Weapon.WeaponType.Missle_launcher)
            {
                GameObject currentDetectedInvader = invaderInViewUpdate(weapons[i]);

                if (currentDetectedInvader != null)
                {
                    StartCoroutine(shootCoroutine(weapons[i]));
                }
            }
            else //when missle launcher
            {
                StartCoroutine(shootCoroutine(weapons[i], getNearestInvader()));
            }
        }
    }

    private GameObject invaderInViewUpdate(ActiveWeapon currentTerranWeapon)
    {
        Vector3 direction = currentTerranWeapon.emitterTransform.up;
        float distance = 20;
        RaycastHit2D[] foundHit = Physics2D.RaycastAll(currentTerranWeapon.emitterTransform.position, direction, distance);
        float toleranceDistance = 0.2f;


        if (foundHit != null && foundHit.Length > 0)
        {
            List<GameObject> allFoundInvaders = new List<GameObject>();

            //search all raycasted invaders
            for (int i = 0; i < foundHit.Length; i++)
            {
                if (foundHit[i].transform.tag == "Invader")
                    allFoundInvaders.Add(foundHit[i].transform.gameObject);
            }
            
            //if invaders raycasted
            if (allFoundInvaders.Count > 0)
            {
                GameObject nearestInvader = MyUtilities.getNearest(allFoundInvaders, transform.position);

                if (nearestInvader != null)
                {
                    ActiveWeapon[] invaderWeapons = nearestInvader.transform.parent.GetComponent<Invader>().weapons;

                    //if invader had weapons
                    if (invaderWeapons != null && invaderWeapons.Length > 0)
                    {
                        //check terran weapon emitter is on the middle x-axe(+-0.2f) from any invader weapon x-axe.
                        for (int invaderWeaponIndex = 0; invaderWeaponIndex < invaderWeapons.Length; invaderWeaponIndex++)
                        {
                            if (invaderWeapons[invaderWeaponIndex].emitterTransform.position.x > currentTerranWeapon.emitterTransform.position.x - toleranceDistance &&
                                invaderWeapons[invaderWeaponIndex].emitterTransform.position.x < currentTerranWeapon.emitterTransform.position.x + toleranceDistance)
                            {
                                Debug.DrawRay(currentTerranWeapon.emitterTransform.position, direction * distance, Color.green);
                                return nearestInvader;
                            }
                        }
                    }
                    else //when invader had no weapons
                    {
                        //if terran weapon emitter is on the same x-axe(+-0.2f) from invader ship x-axe.
                        if (nearestInvader.transform.position.x > currentTerranWeapon.emitterTransform.position.x - toleranceDistance &&
                            nearestInvader.transform.position.x < currentTerranWeapon.emitterTransform.position.x + toleranceDistance)
                        {
                            Debug.DrawRay(currentTerranWeapon.emitterTransform.position, direction * distance, Color.green);
                            return nearestInvader;
                        }
                    }
                }
            }

        }

        Debug.DrawRay(currentTerranWeapon.emitterTransform.position, direction * distance, Color.red);
        return null;

    }

    private GameObject getNearestInvader()
    {
        if (IngameInvaderHandler.instance.getInstantiatedInvaders() == null)
            return null;

        float nearestDistance = 100;
        GameObject nearesInvader = null;

        for (int i = 0; i < IngameInvaderHandler.instance.getInstantiatedInvaders().Count; i++)
        {
            float curDistanceToInvader = Vector3.Distance(transform.position, IngameInvaderHandler.instance.getInstantiatedInvaders()[i].transform.position);
            if (curDistanceToInvader < nearestDistance)
            {
                nearestDistance = curDistanceToInvader;
                nearesInvader = IngameInvaderHandler.instance.getInstantiatedInvaders()[i];
            }
        }

        return nearesInvader;
    }

    private IEnumerator shootCoroutine(ActiveWeapon weapon, GameObject nextTarget = null)
    {
        yield return null;
        if (!isShooting)
        {
            isShooting = true;

            if (nextTarget == null)
                weapon.shoot();
            else
                weapon.missleShoot(nextTarget);

            yield return new WaitForSeconds(0.2f);

            isShooting = false;
        }
    }

    #endregion


    #region Life methods

    public int getCurrentHitpoints()
    {
        return currentHitpoints;
    }

    /*
    * HP - damage
    * return bool: true = destroyed; false = only HP decreasing
    */
    public bool hit(int damageVal)
    {
        if (!isInit)
            return false;

        currentHitpoints -= damageVal;
        TerranStationHandler.instance.sliderHitpointsStation.value = currentHitpoints;
        if (currentHitpoints <= 0)
        {
            currentHitpoints = 0;

            uint calculatedScore = (uint)Mathf.RoundToInt(level * score);
            if (calculatedScore < 1)
                calculatedScore = 1;
            StatsHandler.instance.addCurrentScoreWithFeedback(calculatedScore, "Terran station hit", transform.position);

            ScrapHandler.instance.spawnScrap(transform, Random.Range(minScrapAmount, maxScrapAmount));

            StatsHandler.instance.CurrentKilledStations = 1;

            destroyStation();

            return true;
        }

        return false;
    }

    private void destroyStation()
    {
        MyUtilities.instantiateAndDestroy(explosionEffectPrefab, transform.position, 6);

        StaticAudioHandler.playSound(SoundChooser.instance.explosion_Terran);

        TerranStationHandler.instance.removeStationFromList(gameObject);
    }

    #endregion
}