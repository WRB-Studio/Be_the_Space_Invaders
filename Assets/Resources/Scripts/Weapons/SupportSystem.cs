using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSystem : MonoBehaviour
{
    [Header("Support system infos")]
    public Weapon.WeaponType weaponType = Weapon.WeaponType.None;
    public ActiveWeapon activWeaponData { get; private set; }
    public float effectDuration;
    public float effectDurationCounter;
    public int hitpointsPerSec;
    public int dronesQuantity;

    [Header("Animation settings")]
    private float targetFinderConnectEffectSpeed = 1f;
    private GameObject instantiatedTargetFinderConnectEffect;

    public Invader targetInvaderScrp { get; private set; }

    private Dictionary<GameObject, bool> droneResourcePool;
    private List<GameObject> instantiatedDrones;

    ActiveWeapon[] laserWeapons;
    ActiveWeapon[] bombWeapons;
    ActiveWeapon[] missleWeapons;

    private bool isInit = true;



    public static GameObject instantiateSupportSystem(ActiveWeapon invaderWeaponDataVal, Invader targetInvaderScrpVal = null)
    {
        GameObject newSupportSystem = new GameObject("SupportSystem");
        newSupportSystem.AddComponent<SupportSystem>();

        newSupportSystem.transform.parent = invaderWeaponDataVal.emitterTransform;
        newSupportSystem.transform.localPosition = Vector3.zero;

        newSupportSystem.GetComponent<SupportSystem>().init(invaderWeaponDataVal, targetInvaderScrpVal);

        return newSupportSystem;
    }

    public void init(ActiveWeapon invaderWeaponDataVal, Invader targetInvaderScrpVal = null)
    {
        activWeaponData = invaderWeaponDataVal;

        weaponType = activWeaponData.weaponType;
        effectDuration = activWeaponData.effectDuration;
        effectDurationCounter = effectDuration;
        hitpointsPerSec = activWeaponData.hitpointsPerSec;
        dronesQuantity = activWeaponData.dronesQuantity;

        targetInvaderScrp = targetInvaderScrpVal;

        switch (weaponType)
        {
            case Weapon.WeaponType.Repair_drone:
                StartCoroutine(spawnDronesCoroutine());
                break;
            case Weapon.WeaponType.Collector_drone:
                StartCoroutine(spawnDronesCoroutine());
                break;
            case Weapon.WeaponType.Targeting_system:
                GameObject ps_Connected = Resources.Load<GameObject>("Prefabs/ParticleSystems & Trails/PS_Connected");//is NULL !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                instantiatedTargetFinderConnectEffect = Instantiate(ps_Connected, transform.position, ps_Connected.transform.rotation, transform);
                laserWeapons = targetInvaderScrp.getWeaponsOfType(Weapon.WeaponType.Laser);
                bombWeapons = targetInvaderScrp.getWeaponsOfType(Weapon.WeaponType.Bomb);
                missleWeapons = targetInvaderScrp.getWeaponsOfType(Weapon.WeaponType.Missle_launcher);
                break;
            default:
                break;
        }

        StaticAudioHandler.playSound(SoundChooser.instance.activateSupportSystem);

        isInit = true;
    }

    public static bool canSupport(Invader targetInvaderScrpVal, ActiveWeapon weaponVal)
    {
        //check supportsystem is repair drone and target invader HP is full
        if(weaponVal.weaponType == Weapon.WeaponType.Repair_drone && targetInvaderScrpVal.currentHitpoints >= targetInvaderScrpVal.hitpointsCurrentLevelDefault)
            return false;

        //check is already supported
        SupportSystem[] allSupportSystems = (SupportSystem[])Resources.FindObjectsOfTypeAll(typeof(SupportSystem));
        for (int i = 0; i < allSupportSystems.Length; i++)
        {
            if (allSupportSystems[i].targetInvaderScrp != null && allSupportSystems[i].targetInvaderScrp == targetInvaderScrpVal)
                return false;
        }

        return true;
    }


    public void updateCall()
    {
        if (!isInit || IngameHandler.instance.isPauseOrGameOver())
            return;

        switch (weaponType)
        {
            case Weapon.WeaponType.Repair_drone:
                if (targetInvaderScrp == null)
                {
                    destroySupportSystem();
                    return;
                }

                droneUpdate();
                break;
            case Weapon.WeaponType.Collector_drone:
                droneUpdate();
                break;
            case Weapon.WeaponType.Targeting_system:
                if (targetInvaderScrp == null)
                {
                    destroySupportSystem();
                    return;
                }

                targetingSystemUpdateCall();
                break;
            default:
                break;
        }

        effectDurationCounter -= Time.fixedDeltaTime;
        if (effectDurationCounter <= 0)
            destroySupportSystem();
    }

    private void droneUpdate()
    {
        if (instantiatedDrones == null || instantiatedDrones.Count <= 0)
            return;

        for (int i = 0; i < instantiatedDrones.Count; i++)
        {
            if (instantiatedDrones[i] == null)
                continue;

            instantiatedDrones[i].GetComponent<SupportDrone>().updateCall();
        }
    }

    private void targetingSystemUpdateCall()
    {
        if (laserWeapons != null)
            automaticShotUpdate(laserWeapons);

        if (bombWeapons != null)
            automaticShotUpdate(bombWeapons);

        if (missleWeapons != null)
            automaticMissleUpdate(missleWeapons);

        targetFinderConnectEffectMovement();
    }

    private void automaticShotUpdate(ActiveWeapon[] weaponList)
    {
        for (int weaponIndex = 0; weaponIndex < weaponList.Length; weaponIndex++)
        {
            if (weaponList[weaponIndex].cooldownFinished() && enemyInView(weaponList[weaponIndex]))
            {
                weaponList[weaponIndex].shoot();
            }
        }
    }

    private void automaticMissleUpdate(ActiveWeapon[] weaponList)
    {
        for (int weaponIndex = 0; weaponIndex < weaponList.Length; weaponIndex++)
        {
            if (!weaponList[weaponIndex].cooldownFinished())
                continue;
                        
            GameObject enemyTarget;
            enemyTarget = enemyInView(weaponList[weaponIndex]); //get enemys in front
            if (enemyTarget == null)
                enemyTarget = getNearestEnemy(weaponList[weaponIndex].emitterTransform); //get nearest enemy anywhere

            if (enemyTarget != null)
            {
                GameObject crossHairPrefab = Resources.Load<GameObject>("Prefabs/Weapons/Crosshair");
                GameObject instantiatedCrossHair = Instantiate(crossHairPrefab, enemyTarget.transform.position, crossHairPrefab.transform.rotation);
                instantiatedCrossHair.name = "tmp_Crosshair";

                GameObject missleShoot = weaponList[weaponIndex].missleShoot(instantiatedCrossHair);
                instantiatedCrossHair.GetComponent<Crosshair>().initFollowTargetByMissle(enemyTarget, missleShoot);
            }
        }
    }


    private void targetFinderConnectEffectMovement()
    {
        if (instantiatedTargetFinderConnectEffect == null)
            return;

        float time = Mathf.PingPong(Time.time * targetFinderConnectEffectSpeed, 1);
        instantiatedTargetFinderConnectEffect.transform.position = Vector3.Lerp(transform.position, targetInvaderScrp.transform.GetChild(0).position, time);
    }


    private GameObject enemyInView(ActiveWeapon weapon)
    {
        Vector3 direction = -weapon.emitterTransform.up;
        float distance = 20;
        RaycastHit2D[] foundHit;

        for (float emitterPosition = -0.3f; emitterPosition < 0.3f; emitterPosition += 0.05f)
        {
            Vector3 raycastEmitterPosition = weapon.emitterTransform.position;
            raycastEmitterPosition.x = raycastEmitterPosition.x + emitterPosition;

            foundHit = Physics2D.RaycastAll(raycastEmitterPosition, direction, distance);

            if (foundHit != null && foundHit.Length > 0)
            {
                for (int hitIndex = 0; hitIndex < foundHit.Length; hitIndex++)
                {
                    if (foundHit[hitIndex].transform.tag == "Terran" || foundHit[hitIndex].transform.tag == "Asteroid" || foundHit[hitIndex].transform.tag == "Shoot")
                    {
                        Debug.DrawRay(raycastEmitterPosition, direction * distance, Color.green);
                        return foundHit[hitIndex].transform.gameObject;
                    }
                }
            }
            Debug.DrawRay(raycastEmitterPosition, direction * distance, Color.red);
        }

        return null;
    }

    private GameObject getNearestEnemy(Transform originTransform)
    {
        List<GameObject> allEnemys = new List<GameObject>();
        allEnemys = MyUtilities.addListToList(allEnemys, TerranShipHandler.instance.getInstantiatedTerranShips());
        allEnemys = MyUtilities.addListToList(allEnemys, AsteroidHandler.instance.getInstantiatedAsteroids());

        return MyUtilities.getNearest(allEnemys, originTransform.position);
    }

    private void refreshDroneResourcePool()
    {
        if (droneResourcePool == null)
            droneResourcePool = new Dictionary<GameObject, bool>();

        List<GameObject> asteroidsAndScrap = new List<GameObject>();
        asteroidsAndScrap = MyUtilities.addListToList(asteroidsAndScrap, AsteroidHandler.instance.getInstantiatedAsteroids());
        asteroidsAndScrap = MyUtilities.addListToList(asteroidsAndScrap, ScrapHandler.instance.getInstantiatedScrap());

        for (int i = 0; i < asteroidsAndScrap.Count; i++)
        {
            if (!droneResourcePool.ContainsKey(asteroidsAndScrap[i]))
                droneResourcePool.Add(asteroidsAndScrap[i], false);
        }
    }

    public GameObject getNearestFreeResourceFromPool(Transform sourceTransform)
    {
        refreshDroneResourcePool();

        float nearestObjectDistance = 100;
        GameObject nearestResouce = null;
        foreach (var resourceObject in droneResourcePool)
        {
            if (resourceObject.Key == null || resourceObject.Value)
                continue;

            float currentDistance = Vector3.Distance(sourceTransform.position, resourceObject.Key.transform.position);
            if (currentDistance < nearestObjectDistance)
            {
                nearestObjectDistance = currentDistance;
                nearestResouce = resourceObject.Key;
            }
        }

        if (nearestResouce != null)
        {
            droneResourcePool.Remove(nearestResouce);
            droneResourcePool.Add(nearestResouce, true);
            return nearestResouce;
        }

        return null;
    }


    private IEnumerator spawnDronesCoroutine()
    {
        instantiatedDrones = new List<GameObject>();
        for (int i = 0; i < dronesQuantity; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            GameObject newDrone = null;

            if (weaponType == Weapon.WeaponType.Repair_drone)
            {
                newDrone = Instantiate(Resources.Load<GameObject>("Prefabs/Drones/Drone Repairer"));
            }
            else if (weaponType == Weapon.WeaponType.Collector_drone)
            {
                newDrone = Instantiate(Resources.Load<GameObject>("Prefabs/Drones/Drone Collector"));
            }

            Vector3 spawnPosition = activWeaponData.emitterTransform.position;
            spawnPosition.x = Random.Range(spawnPosition.x - 0.4f, spawnPosition.x + 0.4f);
            spawnPosition.y = Random.Range(spawnPosition.y - 0.4f, spawnPosition.y + 0.4f);
            newDrone.transform.position = spawnPosition;

            newDrone.transform.parent = transform;

            newDrone.GetComponent<SupportDrone>().init(this);

            instantiatedDrones.Add(newDrone);
        }
    }

    public void removeDrone(GameObject droneGO)
    {
        instantiatedDrones.Remove(droneGO);
        Destroy(droneGO);
    }

    public void destroySupportSystem()
    {
        StartCoroutine(destroySupportsystemCoroutine());
    }

    private IEnumerator destroySupportsystemCoroutine()
    {
        yield return null;

        if (instantiatedDrones != null)
        {
            while (instantiatedDrones.Count > 0)
            {
                yield return new WaitForEndOfFrame();

                for (int i = 0; i < instantiatedDrones.Count; i++)
                {
                    yield return new WaitForEndOfFrame();

                    if (i >= instantiatedDrones.Count || instantiatedDrones[i] == null)
                        continue;

                    SupportDrone drone = instantiatedDrones[i].GetComponent<SupportDrone>();
                    drone.canWork = false;
                    if (drone.moveBackToInvaderUpdateCall())
                        removeDrone(instantiatedDrones[i]);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instantiatedDrones != null && instantiatedDrones.Count > 0)
        {
            for (int i = 0; i < instantiatedDrones.Count; i++)
            {
                instantiatedDrones[i].GetComponent<SupportDrone>().destroyWithEffect();
            }
        }
    }

}
