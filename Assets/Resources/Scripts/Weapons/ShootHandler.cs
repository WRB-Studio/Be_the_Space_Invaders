using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootHandler : MonoBehaviour
{
    public GameObject invaderShotStartEffect;
    public GameObject terranShotStartEffect;

    public List<GameObject> instantiatedShoots = new List<GameObject>();

    public static ShootHandler instance;



    private void Awake()
    {
        instance = this;
    }


    private void FixedUpdate()
    {
        if (instantiatedShoots.Count <= 0 || IngameHandler.instance.isPauseOrGameOver())
            return;

        for (int shootsIndex = 0; shootsIndex < instantiatedShoots.Count; shootsIndex++)
        {
            if (instantiatedShoots[shootsIndex] == null)
                continue;

            instantiatedShoots[shootsIndex].GetComponent<Shoot>().movementUpdateCall();
        }
    }


    private void shotStartEffect(bool isInvader, Vector3 spawnPosition)
    {
        Vector3 newPos = spawnPosition;
        GameObject newShotStartEffect = null;

        if (isInvader)
        {
            newShotStartEffect = Instantiate(invaderShotStartEffect);

            newPos.y = newPos.y + 0.1f;
        }
        else
        {
            newShotStartEffect = Instantiate(terranShotStartEffect);

            newPos.y = newPos.y - 0.1f;
        }

        newPos.z = 0;
        newShotStartEffect.transform.position = newPos;

        Destroy(newShotStartEffect, 5);
    }

    public void instantiateShoot(ActiveWeapon invaderWeapondata, Vector3 emitterPosition)
    {
        shotStartEffect(invaderWeapondata.isInvaderWeapon, emitterPosition);

        GameObject newShoot;
        if (invaderWeapondata.isInvaderWeapon)
            newShoot = Instantiate(invaderWeapondata.weapon.GetComponent<Weapon>().shootPrefabInvader, transform);
        else
            newShoot = Instantiate(invaderWeapondata.weapon.GetComponent<Weapon>().shootPrefabTerrans, transform);

        emitterPosition.z = 0;
        newShoot.transform.position = emitterPosition;

        newShoot.GetComponent<Shoot>().init(invaderWeapondata);
        instantiatedShoots.Add(newShoot);
    }

    public GameObject instantiateMissleShoot(ActiveWeapon invaderWeapondata, Vector3 emitterPosition, GameObject targetObject)
    {
        shotStartEffect(invaderWeapondata.isInvaderWeapon, emitterPosition);

        GameObject newShoot;
        if (invaderWeapondata.isInvaderWeapon)
            newShoot = Instantiate(invaderWeapondata.weapon.GetComponent<Weapon>().shootPrefabInvader, transform);
        else
            newShoot = Instantiate(invaderWeapondata.weapon.GetComponent<Weapon>().shootPrefabTerrans, transform);

        emitterPosition.z = 0;
        newShoot.transform.position = emitterPosition;

        newShoot.GetComponent<Shoot>().initMissleShoot(invaderWeapondata, targetObject);
        instantiatedShoots.Add(newShoot);

        return newShoot;
    }


    public void refreshMissileTargetsByDestroyedStation()
    {
        for (int i = 0; i < instantiatedShoots.Count; i++)
        {
            instantiatedShoots[i].GetComponent<Shoot>().refreshMissileTargetsByDestroyedStation();
        }
    }

    public List<GameObject> getInstantiatedShoots()
    {
        return instantiatedShoots;
    }

    public List<GameObject> getInstantiatedTerranShoots()
    {
        List<GameObject> filteredList = new List<GameObject>();
        for (int i = 0; i < instantiatedShoots.Count; i++)
        {
            if (!instantiatedShoots[i].GetComponent<Shoot>().isFromInvader)
                filteredList.Add(instantiatedShoots[i]);
        }
        return filteredList;
    }


    public void removeShootFromList(GameObject removingObject)
    {
        MyUtilities.removeAndDestroyGameObjectFromList(ref instantiatedShoots, removingObject);
    }

    public void removeShootFromListAfterTime(GameObject removingObject, float destroyDelay)
    {
        MyUtilities.removeAndDestroyGameObjectFromListAfterTime(ref instantiatedShoots, removingObject, destroyDelay);
    }
}
