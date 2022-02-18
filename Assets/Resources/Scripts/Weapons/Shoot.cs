using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{

    [Header("Shoot infos")]
    ActiveWeapon weaponData;
    public Weapon.WeaponType weaponType = Weapon.WeaponType.None;
    public float missleMaxLifeTime = 10;
    private float noTargetTime = 3;
    private float currentNoTargetCountdown = 2;
    private float speed;
    public int damage { get; private set; }
    public bool isFromInvader { get; private set; }

    [Header("Gain settings")]
    public uint score;

    [Header("Crosshair prefab")]
    private GameObject crossHairPrefab;
    private GameObject instantiatedCrossHair;

    [Header("Animation settings")]
    public GameObject invaderMissleBombExplosionPrefab;
    public GameObject terranMissleBombExplosionPrefab;

    private Transform target;
    private bool isStationTarget = false;
    private bool isInit = false;
    private ActiveWeapon activeWeaponEmitter;
    private bool hitted = false;
    private bool outOfDisplay = false;


    private void Awake()
    {
        crossHairPrefab = Resources.Load<GameObject>("Prefabs/Weapons/CrossHair");
        currentNoTargetCountdown = noTargetTime;
    }

    public void init(ActiveWeapon weapondataVal)
    {
        weaponData = weapondataVal;

        activeWeaponEmitter = weaponData;
        isFromInvader = weaponData.isInvaderWeapon;
        damage = weaponData.damage;
        speed = weaponData.speed;

        if (isFromInvader)
        {
            transform.Rotate(0, 0, 180);
            if (weaponType == Weapon.WeaponType.Bomb)
                speed = -speed;
        }

        playStartSound();

        isInit = true;
    }

    public void initMissleShoot(ActiveWeapon weapondataVal, GameObject targetObject)
    {
        weaponData = weapondataVal;

        isFromInvader = weapondataVal.isInvaderWeapon;
        damage = weapondataVal.damage;
        speed = weapondataVal.speed;

        if (isFromInvader)
            transform.Rotate(0, 0, 180);

        target = null;
        if (targetObject != null)
        {
            target = targetObject.transform;
            setCrosshair(target.gameObject);

            //check missile target is Terran station
            if (weaponType == Weapon.WeaponType.Missle_launcher)
            {
                GameObject station = TerranStationHandler.instance.getInstantiatedTerranStation();
                if (station != null && target.name == "tmp_missileTarget_waypoint")
                {
                    Collider2D tmpTargetCollider = target.gameObject.AddComponent<BoxCollider2D>();
                    if (station.GetComponent<Collider2D>().bounds.Intersects(tmpTargetCollider.bounds))
                    {
                        isStationTarget = true;
                    }
                }
            }
        }

        playStartSound();

        isInit = true;
    }

    private void setCrosshair(GameObject targetGO)
    {
        instantiatedCrossHair = Instantiate(crossHairPrefab, targetGO.transform.position, crossHairPrefab.transform.rotation);
        instantiatedCrossHair.name = "tmp_Crosshair";
        Crosshair crossHairScrp = instantiatedCrossHair.GetComponent<Crosshair>();
        crossHairScrp.initFollowTargetByMissle(targetGO, gameObject);
    }

    public void movementUpdateCall()
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                laserMovement();
                break;
            case Weapon.WeaponType.Missle_launcher:
                if (isFromInvader)
                    missleHandlingByInvader();
                else
                    missleHandlingByTerran();
                break;
            case Weapon.WeaponType.Bomb:
                bombMovement();
                break;
            default:
                break;
        }

        destroyWhenOutOfDisplay();
    }

    private void laserMovement()
    {
        transform.Translate(0, speed * Time.fixedDeltaTime, 0);
    }


    private void missleHandlingByInvader()
    {
        //handle lifetime
        missleMaxLifeTime -= Time.fixedDeltaTime;
        if (missleMaxLifeTime <= 0 || currentNoTargetCountdown <= 0)
            explosion();

        if (target == null)//search for next target
        {
            GameObject newTarget = getNextMissileTarget();

            if (newTarget != null)
            {
                target = newTarget.transform;
                currentNoTargetCountdown = noTargetTime;

                setCrosshair(target.gameObject);
            }

            if (target == null && currentNoTargetCountdown > 0)
                currentNoTargetCountdown -= Time.fixedDeltaTime;
        }

        if (target != null && target.name == "tmp_missileTarget_waypoint" && Vector3.Distance(transform.position, target.position) < 0.05f) //explode when near target
            explosion();


        missleMovement(target);
    }

    private void missleHandlingByTerran()
    {
        //handle lifetime
        missleMaxLifeTime -= Time.fixedDeltaTime;
        if (missleMaxLifeTime <= 0 || currentNoTargetCountdown <= 0)
            explosion();

        if (target == null)
        {
            GameObject newTarget = MyUtilities.getNearest(IngameInvaderHandler.instance.getInstantiatedInvaders(), transform.position);

            if (newTarget != null)
            {
                target = newTarget.transform;
                currentNoTargetCountdown = noTargetTime;
            }

            if (target == null && currentNoTargetCountdown > 0)
                currentNoTargetCountdown -= Time.fixedDeltaTime;
        }

        missleMovement(target);
    }

    private bool missleMovement(Transform targetTransform)
    {
        if (targetTransform != null && !outOfDisplay)
        {
            //missle face to target
            Vector3 diff = targetTransform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

            //move to target
            transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
        }
        else if (targetTransform == null)
        {
            //move
            transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
        }

        return true;
    }


    private void bombMovement()
    {
        transform.Translate(0, speed * Time.fixedDeltaTime, 0, Space.World);
    }


    private void destroyWhenOutOfDisplay()
    {
        //no collision when out of display
        if (MyUtilities.objectOutOfCameraBounds(Camera.main, 0.2f, gameObject))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        if (MyUtilities.objectOutOfCameraBounds(Camera.main, 0.5f, gameObject))
            outOfDisplay = true;

        if (MyUtilities.objectOutOfCameraBounds(Camera.main, 1f, gameObject))
            StartCoroutine(destroyShootCoroutine(false));

    }

    private GameObject getNextMissileTarget()
    {
        GameObject newTarget = null;

        newTarget = TerranStationHandler.instance.getInstantiatedTerranStation();//get terran station
        if (newTarget == null)//no terran station exists -> check for other targets
        {
            List<GameObject> possibleTargets = null;

            possibleTargets = TerranShipHandler.instance.getInstantiatedTerranShips();//get terran ships

            if (possibleTargets == null || possibleTargets.Count <= 0)//no terran ships found, check for terran shoots
                possibleTargets = ShootHandler.instance.getInstantiatedTerranShoots();//get terran shoots

            if (possibleTargets == null || possibleTargets.Count <= 0)//no terran shoots found, check for asteroids
                possibleTargets = AsteroidHandler.instance.getInstantiatedAsteroids();//get asteroids

            newTarget = MyUtilities.getNearest(possibleTargets, transform.position);//get neares target
        }
        else if (newTarget.GetComponent<TerranStation>() != null && newTarget.GetComponent<TerranStation>().getIsInit())
        {
            GameObject stationTarget = newTarget;

            newTarget = new GameObject("tmp_missileTarget_waypoint");
            newTarget.transform.position = new Vector3(Random.Range(stationTarget.transform.position.x - 1.5f, stationTarget.transform.position.x + 1.5f),
                                                        stationTarget.transform.position.y,
                                                        stationTarget.transform.position.z);

            isStationTarget = true;
        }

        return newTarget;
    }


    public void onDeflectorShieldCollision()
    {
        if (weaponType != Weapon.WeaponType.Laser && weaponType != Weapon.WeaponType.Scattering_laser)
            return;

        if (isFromInvader)
            isFromInvader = false;
        else
            isFromInvader = true;

        transform.Rotate(0, 0, 180);
    }


    public void refreshMissileTargetsByDestroyedStation()
    {
        if (isStationTarget)
        {
            isStationTarget = false;
            if (weaponType == Weapon.WeaponType.Missle_launcher)
            {
                Destroy(target.gameObject, 0.1f);
                target = null;
            }
        }
    }


    public void hit()
    {
        destroyShoot();
    }

    private void explosion()
    {
        destroyShoot();
    }

    public void destroyShoot(Transform collisionTransfom = null)
    {
        StartCoroutine(destroyShootCoroutine(true, collisionTransfom));
    }

    private IEnumerator destroyShootCoroutine(bool playSound = true, Transform collisionTransfom = null, float delay = 0f)
    {
        yield return new WaitForSeconds(0.05f);

        if (collisionTransfom != null)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.15f));
            if (collisionTransfom != null)
                DamageParticleHandler.setDamageParticle(transform.position, collisionTransfom);
        }

        if (target != null && target.name == "tmp_missileTarget_waypoint")
            Destroy(target.gameObject);

        if (weaponType == Weapon.WeaponType.Missle_launcher || weaponType == Weapon.WeaponType.Bomb)
        {
            if (isFromInvader)
                MyUtilities.instantiateAndDestroy(invaderMissleBombExplosionPrefab, transform.position, 4);
            else
                MyUtilities.instantiateAndDestroy(terranMissleBombExplosionPrefab, transform.position, 4);
        }

        if (playSound)
            playImpactSound();

        yield return new WaitForSeconds(delay);

        ShootHandler.instance.removeShootFromList(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isInit)
            return;

        GameObject colliderGO = col.collider.gameObject;

        if (isFromInvader) //if shoot came from invader
        {
            if (colliderGO.tag == "Shoot" && !colliderGO.GetComponent<Shoot>().isFromInvader)
            {
                StatsHandler.instance.addCurrentScoreWithFeedback(score, "Shoot defended", transform.position);
                StatsHandler.instance.addTotalShootKills(1);
                shotHitted();

                colliderGO.GetComponent<Shoot>().hit();

                destroyShoot();
            }
            else if (colliderGO.tag == "Asteroid")
            {
                if (MyUtilities.objectOutOfCameraBounds(Camera.main, 0.2f, colliderGO.gameObject))// check asteroid is in camera view
                    return;

                colliderGO.GetComponent<Asteroid>().hit(damage);
                destroyShoot();
            }
            else if (colliderGO.tag == "Terran")
            {
                shotHitted();

                if (colliderGO.GetComponent<TerranShip>().hit(damage))
                {
                    StatsHandler.instance.addKillByInvaderType(weaponData.invaderScrp.name);
                    destroyShoot();
                }
                else
                    destroyShoot(col.transform);
            }
            else if (colliderGO.tag == "Station")
            {
                if (colliderGO.GetComponent<TerranStation>().hit(damage))
                {
                    destroyShoot();
                    StatsHandler.instance.addKillByInvaderType(weaponData.invaderScrp.name);
                }
                else
                {
                    shotHitted();
                    destroyShoot(col.transform);
                }
            }
        }
        else if (!isFromInvader) //if shoot came from terran
        {
            if (colliderGO.gameObject.tag == "Invader")
            {
                StatsHandler.instance.addDefencesByInvaderType(colliderGO.gameObject.name);

                if (colliderGO.GetComponent<InvaderColliderHandler>().hit(damage))
                    destroyShoot();
                else
                    destroyShoot(col.transform);
            }
        }
    }

    private void shotHitted()
    {
        if (!hitted)
        {
            StatsHandler.instance.addShotHits(1);
            hitted = true;
        }
    }


    private void playStartSound()
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                if (isFromInvader)
                    StaticAudioHandler.playSound(SoundChooser.instance.laserShot_Invader);
                else
                    StaticAudioHandler.playSound(SoundChooser.instance.laserShot_Terran);
                break;
            case Weapon.WeaponType.Missle_launcher:
                if (isFromInvader)
                    StaticAudioHandler.playSound(SoundChooser.instance.missileShot_Invader);
                else
                    StaticAudioHandler.playSound(SoundChooser.instance.missileShot_Terran);
                break;
            case Weapon.WeaponType.Bomb:
                if (isFromInvader)
                    StaticAudioHandler.playSound(SoundChooser.instance.bombShot_Invader);
                else
                    StaticAudioHandler.playSound(SoundChooser.instance.bombShot_Terran);
                break;
            default:
                break;
        }
    }

    private void playImpactSound()
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                StaticAudioHandler.playSound(SoundChooser.instance.laserHit);
                break;
            case Weapon.WeaponType.Missle_launcher:
                StaticAudioHandler.playSound(SoundChooser.instance.missileHit);
                break;
            case Weapon.WeaponType.Bomb:
                StaticAudioHandler.playSound(SoundChooser.instance.bombHit);
                break;
            default:
                break;
        }
    }

}
