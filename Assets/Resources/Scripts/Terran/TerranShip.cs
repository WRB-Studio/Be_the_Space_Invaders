using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerranShip : MonoBehaviour
{
    public enum TerranShipClass
    {
        None, Small_ship, Frigate, Battleship
    }

    [Header("Ship infos")]
    public int id;
    public TerranShipClass shipClass = TerranShipClass.None;
    public int level;
    public int hitpoints;
    private int currentHitpoints;

    [Header("Movement settings")]
    public float maxSpeed;
    public float speed;
    public float rotationSpeed;
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
    private GameObject currentWaypoint;



    #region general methods

    public void init(int currentStage)
    {
        currentHitpoints = hitpoints;

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].init();

        levelUp(currentStage);

        isInit = true;
    }

    public void updateCall()
    {
        if (!isInit || weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        survivedTime += Time.fixedDeltaTime;

        weaponUpdate();

        movement();
    }

    public void levelUp(int currentStage)
    {
        level = currentStage;

        speed = speed + ((maxSpeed / 100) * level);
        if (speed > maxSpeed)
            speed = maxSpeed;

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

    #endregion


    #region Movement methods

    private void movement()
    {
        if (currentWaypoint != null && transform.position != currentWaypoint.transform.position)
        {
            switch (shipClass)
            {
                case TerranShipClass.None:
                    break;
                case TerranShipClass.Small_ship:
                    movementFighter();
                    break;
                case TerranShipClass.Frigate:
                    movementFrigate();
                    break;
                case TerranShipClass.Battleship:
                    movementBattleship();
                    break;
                default:
                    break;
            }
        }
        else if (currentWaypoint == null || transform.position == currentWaypoint.transform.position)
        {
            setWaypoint();
        }
    }

    private void movementFighter()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, Time.fixedDeltaTime * speed);
    }

    private void movementFrigate()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, Time.fixedDeltaTime * speed);
    }

    private void movementBattleship()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, Time.fixedDeltaTime * speed);


        /*Vector3 targetDir = currentWaypoint.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/

        /*
        //movement with rotation
        Vector3 dir = currentWaypoint.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.time);

        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);

        //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, Time.fixedDeltaTime * speed);
        */
    }

    private void setWaypoint()
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = new GameObject("tmp_TerranWaypoint");
            currentWaypoint.transform.parent = TerranShipHandler.instance.transform;
        }

        //set random position
        Vector3 displayDimension = MyUtilities.getOrthographicCameraBounds(Camera.main).extents;
        if (Random.value <= 0.3f)
            currentWaypoint.transform.position = new Vector3(Random.Range(-displayDimension.x + 0.2f, displayDimension.x - 0.2f), Random.Range(-displayDimension.y + 0.2f, -displayDimension.y / 3), 0);
        else
            currentWaypoint.transform.position = new Vector3(Random.Range(-displayDimension.x + 0.2f, displayDimension.x - 0.2f), transform.position.y, 0);


        //check in terran movement direction (X-Axe) is a invader ship, else direction.x /= -1;
        List<GameObject> allInvaders = IngameInvaderHandler.instance.getInstantiatedInvaders();
        for (int i = 0; i < allInvaders.Count; i++)
        {
            if (transform.position.x < currentWaypoint.transform.position.x) //terran move directino to right
            {
                if (allInvaders[i].transform.position.x >= transform.position.x)//found invader in move direction
                    return;
            }
            else if (transform.position.x > currentWaypoint.transform.position.x) //terran move directino to left
            {
                if (allInvaders[i].transform.position.x <= transform.position.x)//found invader in move direction
                    return;
            }
        }
        //no invader in movement direction found => change move direction trough set new waypoint
        currentWaypoint.transform.position = new Vector3(-currentWaypoint.transform.position.x, currentWaypoint.transform.position.y, currentWaypoint.transform.position.z);
    }

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

            if (isShooting || !weapons[i].cooldownFinished())//check ship is shooting and weapon cooldown is finished
            {
                continue;
            }

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
        float toleranceDistance = 0.01f;


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

            //next target available => missle launcher shoot
            if (nextTarget != null)
                weapon.missleShoot(nextTarget);
            else // when no missle launcher => normal shoot
                weapon.shoot();

            yield return new WaitForSeconds(0.2f);

            isShooting = false;
        }
    }

    #endregion


    #region Life methods

    /*
    * HP - damage
    * return bool: true = destroyed; false = only HP decreasing
    */
    public bool hit(int damageVal)
    {
        if (!isInit)
            return false;

        currentHitpoints -= damageVal;
        if (currentHitpoints <= 0)
        {
            currentHitpoints = 0;

            uint calculatedScore = (uint)Mathf.RoundToInt(level * score);
            if (calculatedScore < 1)
                calculatedScore = 1;
            StatsHandler.instance.addCurrentScoreWithFeedback(calculatedScore, "Terran ship (" + name + ") hit", transform.position);

            ScrapHandler.instance.spawnScrap(transform, Random.Range(minScrapAmount, maxScrapAmount));
            StatsHandler.instance.CurrentKilledTerrans = 1;

            switch (shipClass)
            {
                case TerranShipClass.Small_ship:
                    StatsHandler.instance.addTotalKilledTerranFighters(1);
                    break;
                case TerranShipClass.Frigate:
                    StatsHandler.instance.addTotalKilledTerranFrigates(1);
                    break;
                case TerranShipClass.Battleship:
                    StatsHandler.instance.addTotalKilledTerranBattleships(1);
                    break;
                default:
                    break;
            }

            destroyTerran();

            return true;
        }

        return false;
    }

    private void destroyTerran()
    {
        MyUtilities.instantiateAndDestroy(explosionEffectPrefab, transform.position, 6);

        StaticAudioHandler.playSound(SoundChooser.instance.explosion_Terran);

        TerranShipHandler.instance.removeInvaderFromList(gameObject);
    }

    #endregion

}
