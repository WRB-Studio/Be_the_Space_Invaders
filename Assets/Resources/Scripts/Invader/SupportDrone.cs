using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportDrone : MonoBehaviour
{
    public enum DroneType
    {
        None, Repairing_drone, Collector_drone
    }

    [Header("Drone infos")]
    public DroneType droneType = DroneType.None;

    [Header("Animation settings")]
    public GameObject repairEffectPrefab;
    public GameObject collectEffectPrefab;
    public GameObject explosionPrefab;

    private float moveSpeed;
    private float idlePositionDistance;
    private float aroundSpeed;

    private SupportSystem supportSystemScrpSource;
    private Transform startPointTransform;
    private Transform target;
    private Transform droneEmitter;

    private bool isRepairing = false;
    public bool canWork { get; set; }



    public void init(SupportSystem supportSystemScrpVal)
    {
        droneEmitter = transform.GetChild(0);

        supportSystemScrpSource = supportSystemScrpVal;
        moveSpeed = supportSystemScrpVal.activWeaponData.speed;
        startPointTransform = supportSystemScrpVal.transform;

        if (droneType == DroneType.Repairing_drone)
            target = supportSystemScrpVal.targetInvaderScrp.transform.GetChild(0);

        idlePositionDistance = Random.Range(0.4f, 0.7f);
        aroundSpeed = Random.Range(5, 25);
        if (Random.value > 0.5f)
            aroundSpeed = -aroundSpeed;

        canWork = true;
    }


    public void updateCall()
    {
        if (!canWork)
            return;

        if (droneType == DroneType.Repairing_drone)
            repairDroneHandling();
        else if (droneType == DroneType.Collector_drone)
            collectorDroneHandling();
    }

    private void repairDroneHandling()
    {
        if (target != null)
        {
            if (!isRepairing && Vector3.Distance(target.position, transform.position) > 0.4f) //fly to target
            {
                MyUtilities.moveToTargetByFacing(transform, target, moveSpeed);
            }
            else if (!isRepairing && Vector3.Distance(target.position, transform.position) <= 0.4f) //start move around target position
            {
                if (!isRepairing)
                {
                    StartCoroutine(repairPerSecondCoroutine());
                    StartCoroutine(repairMoveAroundCoroutine());
                }
            }
        }
        else
        {
            Destroy(supportSystemScrpSource.gameObject);
        }
    }

    private void collectorDroneHandling()
    {
        if (target == null) //try to find new resource target
        {
            GameObject newTarget = supportSystemScrpSource.getNearestFreeResourceFromPool(transform);
            if (newTarget != null)
                target = newTarget.transform;
        }

        if (target != null) //resource target available => fly to resource target and collect them
        {
            if (Vector3.Distance(target.position, transform.position) > 0.2f) //fly to resource
            {
                MyUtilities.moveToTargetByFacing(transform, target, moveSpeed);
            }
            else
            {
                //collect animation
                MyUtilities.instantiateAndDestroy(collectEffectPrefab, droneEmitter.position, 2);

                if (target.tag == "Scrap")//collect scrap
                {
                    StatsHandler.instance.addCurrentScoreWithFeedback(target.GetComponent<Scrap>().score, "Collector drone", transform.position);
                    uint collectedResources = target.GetComponent<Scrap>().scrapGain * supportSystemScrpSource.activWeaponData.resourceExtraction;
                    uint gainedRes = StatsHandler.instance.addCurrentCollectedResources(collectedResources, true, true, target.transform.position);
                    StatsHandler.instance.addResGainByInvaderType(supportSystemScrpSource.activWeaponData.invaderScrp.name, gainedRes);

                    target.GetComponent<Scrap>().destroyScrap(true);
                }
                else if (target.tag == "Asteroid")//collect Asteroid
                {
                    target.gameObject.GetComponent<Asteroid>().hit(1, false);

                    return;
                }
            }
        }
        else //no resource target found 
        {
            if (Vector3.Distance(transform.position, Vector2.zero) > idlePositionDistance) //fly to idle position
            {
                MyUtilities.moveToPositionByFacing(transform, Vector2.zero, moveSpeed);
            }
            else //idle movement
            {
                MyUtilities.rotateAroundPosition(transform, Vector2.zero, aroundSpeed, false);
                MyUtilities.faceToPosition(transform, new Vector2(0, -5));
            }

            //fly back to invader and move around it
            /*if (Vector3.Distance(transform.position, startPointTransform.position) > rotateAroundDistance) //fly to invader
            {
                MyUtilities.moveToTargetByFacing(transform, startPointTransform, moveSpeed);
            }
            else //move around invader
            {
                MyUtilities.moveAroundTarget(transform, startPointTransform, aroundSpeed, true);
            }*/
        }
    }

    public bool moveBackToInvaderUpdateCall()
    {
        if (Vector3.Distance(transform.position, supportSystemScrpSource.transform.position) > idlePositionDistance / 2) //fly to invader
        {
            MyUtilities.moveToTargetByFacing(transform, supportSystemScrpSource.transform, moveSpeed / 20);
            return false;
        }
        else
        {
            return true;
        }
    }


    private IEnumerator repairMoveAroundCoroutine()
    {
        Vector3 direction = Vector3.up;
        float randomDistance = Random.Range(0.3f, 0.5f);

        yield return new WaitForSeconds(Random.Range(0, 0.04f));
        float randomAroundSpeed = Random.Range(5, 25);
        if (Random.value > 0.5)
            randomAroundSpeed = -randomAroundSpeed;

        float nearestDistance = Random.Range(0.2f, 0.3f);
        float farestDistance = Random.Range(0.3f, 0.4f);

        while (true)
        {
            yield return null;

            MyUtilities.rotateAroundTarget(transform, target, randomAroundSpeed, true);
            transform.Translate(direction * 0.05f * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, target.position) < randomDistance)
            {
                direction = -Vector3.up;
                randomDistance = Random.Range(randomDistance, farestDistance);
            }
            else if (Vector3.Distance(transform.position, target.position) > randomDistance)
            {
                direction = Vector3.up;
                randomDistance = Random.Range(nearestDistance, randomDistance);
            }
        }
    }

    private IEnumerator repairPerSecondCoroutine()
    {
        isRepairing = true;

        //collect animation
        GameObject newCollectEffect = Instantiate(repairEffectPrefab, droneEmitter.position, collectEffectPrefab.transform.rotation, droneEmitter);
        Invader targetInvaderScrp = target.parent.GetComponent<Invader>();

        while (true)
        {
            yield return new WaitForSeconds(1);

            if (targetInvaderScrp.currentHitpoints < targetInvaderScrp.hitpointsCurrentLevelDefault)
            {
                newCollectEffect.gameObject.SetActive(true);
                targetInvaderScrp.repairing(supportSystemScrpSource.hitpointsPerSec);
            }
            else
            {
                newCollectEffect.gameObject.SetActive(false);
            }
        }
    }


    public void destroyWithEffect()
    {
        MyUtilities.instantiateAndDestroy(explosionPrefab, transform.position, 2);
        supportSystemScrpSource.removeDrone(gameObject);
    }

}
