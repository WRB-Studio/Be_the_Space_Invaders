using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private GameObject target;
    private GameObject missle;
    private bool followTarget = false;
    private bool followTargetByMissle = false;



    public void initFollowTargetByMissle(GameObject newTarget, GameObject missleVal)
    {
        target = newTarget;
        missle = missleVal;

        followTargetByMissle = true;
        followTarget = true;
    }

    public void initFollowTargetBySupportSystem(GameObject newTarget)
    {
        target = newTarget;

        followTarget = true;
    }

    private void FixedUpdate()
    {
        if (!followTarget)
            return;

        followTargetUpdate();
    }

    private void followTargetUpdate()
    {
        if (followTargetByMissle && (target == null || missle == null))
        {
            Destroy(gameObject);
            return;
        }
        else if (!followTargetByMissle && target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = target.transform.position;
    }

    public void followPositionUpdate(Vector3 position)
    {
        transform.position = position;
    }

}
