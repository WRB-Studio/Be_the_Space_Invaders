using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Stats")]
    public int hitpoints;
    public int damage;

    [Header("Color & transform settings")]
    public Color[] textureColor;
    public float minScale;
    public float maxScale;

    [Header("Movement settings")]
    public float minSpeed;
    public float maxSpeed;
    public float speed { get; private set; }
    public float minRotationSpeed;
    public float maxRotationSpeed;
    private float rotationSpeed;

    [Header("Gain settings")]
    public uint score;
    public int minScrapAmount = 1;
    public int maxScrapAmount = 4;
    private int resGain = 1;

    [Header("Animation settings")]
    public GameObject explosionPrefab;

    private Vector2 targetPosition;



    public void init(Vector2 newTargetPosition, int resGainVal)
    {
        targetPosition = newTargetPosition;

        resGain = resGainVal;

        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        speed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value < 0.5f)
            rotationSpeed /= -1;


        Sprite[] asteroidTextures = Resources.LoadAll<Sprite>("Sprite/Asteroids");
        GetComponent<SpriteRenderer>().sprite = asteroidTextures[Random.Range(0, asteroidTextures.Length)];
        GetComponent<SpriteRenderer>().color = textureColor[Random.Range(0, textureColor.Length)];

        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void updateCall()
    {
        transform.Rotate(0, 0, rotationSpeed);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed);

        if (MyUtilities.objectOutOfCameraBounds(Camera.main, 1, gameObject))
            destroyAsteroid();
    }

    /*
    * HP - damage
    * return bool: true = destroyed; false = only HP decreasing
    */
    public bool hit(int damage, bool explosionAnim = true)
    {
        hitpoints -= damage;

        if (hitpoints <= 0)
        {
            StatsHandler.instance.addCurrentScoreWithFeedback(score, "Asteroid", transform.position);
            StatsHandler.instance.CurrentKilledAsteroids = 1;
            ScrapHandler.instance.spawnScrap(transform, Random.Range(minScrapAmount, maxScrapAmount));

            destroyAsteroid(explosionAnim);
            return true;
        }

        return false;
    }

    public void destroyAsteroid(bool explosionAnim = true)
    {
        if (explosionAnim)
            MyUtilities.instantiateAndDestroy(explosionPrefab, transform.position, 6);

        if (transform.childCount > 0)
            MyUtilities.destroyParticleSystem(transform.GetChild(0).gameObject);

        StaticAudioHandler.playSound(SoundChooser.instance.explosion_Asteroid);

        AsteroidHandler.instance.removeAsteroidFromList(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject colliderGO = col.collider.gameObject;

        if (colliderGO.tag == "Invader")
        {
            Invader invaderScrp = colliderGO.transform.parent.GetComponent<Invader>();

            if (invaderScrp.name == "Scrap Crusher")
            {
                ScrapHandler.instance.spawnScrap(transform, Random.Range(minScrapAmount, maxScrapAmount));

                StatsHandler.instance.addDefencesByInvaderType(invaderScrp.name);
            }

            invaderScrp.hit(damage);

            //DamageParticleHandler.setDamageParticle(col.contacts[0].point, colliderGO.transform);

            destroyAsteroid();
        }
    }
}
