using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticleHandler : MonoBehaviour
{

    public static void setDamageParticle(Vector2 collisionPoint, Transform parent = null)
    {
        if (parent != null && parent.tag == "Invader" || parent.tag == "Terran" || parent.tag == "Station")
        {
            Transform damageParticleParent = parent.Find("DamageParticles");
            if (damageParticleParent == null)
            {
                damageParticleParent = new GameObject("DamageParticles").transform;
                damageParticleParent.parent = parent;
                damageParticleParent.localPosition = Vector3.zero;
            }
            parent = damageParticleParent;
        }

        GameObject newDamageParticle = Instantiate(Resources.Load<GameObject>("Prefabs/DamageParticles/DamageParticle01"), collisionPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), parent);

        float randomScale = Random.Range(newDamageParticle.transform.localScale.x, newDamageParticle.transform.localScale.x + 0.05f);
        newDamageParticle.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        Sprite[] damageParticleSprites = Resources.LoadAll<Sprite>("Sprite/DamageParticles");
        newDamageParticle.GetComponent<SpriteMask>().sprite = damageParticleSprites[Random.Range(0, damageParticleSprites.Length)];

        MyUtilities.instantiateAndDestroy(Resources.Load<GameObject>("Prefabs/ParticleSystems & Trails/Weapons/PS_ShootImpact"), collisionPoint, 1);

        if (parent != null && parent.parent.tag == "Invader" || parent.parent.tag == "Terran" || parent.parent.tag == "Station")
        {
            newDamageParticle.GetComponent<SpriteMask>().frontSortingOrder = parent.parent.GetComponent<SpriteRenderer>().sortingOrder;
            newDamageParticle.GetComponent<SpriteMask>().backSortingOrder = parent.parent.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }

}
