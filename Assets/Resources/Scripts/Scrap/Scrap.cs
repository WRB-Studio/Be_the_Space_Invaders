using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    public float lifeTime;
    private float currentLifeTime;
    public float collectAfterTime = 0.5f;
    private bool canCollect = false;

    [Header("Gain settings")]
    public uint score;
    public uint scrapGain = 1;

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

    [Header("Animations")]
    public GameObject destroyEffectPrefab;

    private Vector2 targetPosition;



    private void Start()
    {
        currentLifeTime = lifeTime;
    }

    public void init(Transform sourceTransform)
    {
        targetPosition = (transform.position - sourceTransform.position) * 100;

        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        speed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value < 0.5f)
            rotationSpeed /= -1;

        Sprite[] asteroidTextures = Resources.LoadAll<Sprite>("Sprite/Resources/current");
        GetComponent<SpriteRenderer>().sprite = asteroidTextures[Random.Range(0, asteroidTextures.Length)];
        GetComponent<SpriteRenderer>().color = textureColor[Random.Range(0, textureColor.Length)];

        canCollect = false;

        gameObject.AddComponent<CircleCollider2D>();
    }


    public void updateCall()
    {
        lifeTimeColorUpdate();

        movementUpdate();       

        if (MyUtilities.objectOutOfCameraBounds(Camera.main, 0.5f, gameObject))
            destroyScrap(false);
    }

    private void lifeTimeColorUpdate()
    {
        currentLifeTime -= Time.fixedDeltaTime;
        if (currentLifeTime < lifeTime / 3)
        {
            Color tmpColor = GetComponent<SpriteRenderer>().color;
            tmpColor.a -= Time.fixedDeltaTime * 0.5f;
            GetComponent<SpriteRenderer>().color = tmpColor;

            if (tmpColor.a <= 0)
                destroyScrap(false);
        }
    }

    private void movementUpdate()
    {
        if(currentLifeTime > lifeTime - collectAfterTime)
        {
            transform.Rotate(0, 0, rotationSpeed);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed * 2);
        }
        else
        {
            canCollect = true;
            transform.Rotate(0, 0, rotationSpeed);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed);
        }
        
    }

    public void destroyScrap(bool destroyCollecting)
    {
        if (transform.childCount > 0)
            MyUtilities.destroyParticleSystem(transform.GetChild(0).gameObject);

        ScrapHandler.instance.removeScrapFromList(gameObject, destroyCollecting);
    }


    private void OnMouseEnter()
    {
        if (!canCollect)
            return;

        ScrapHandler.instance.addMultiplier(score, transform.position);
        //StatsHandler.instance.addCurrentScore(score, "Scrap collected by tap");
        uint gainedRes = StatsHandler.instance.addCurrentCollectedResources(scrapGain, true, true, transform.position);
        StatsHandler.instance.addManualResGain(gainedRes);

        MyUtilities.instantiateAndDestroy(destroyEffectPrefab, transform.position, 6);

        destroyScrap(true);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Invader" && col.gameObject.transform.parent.GetComponent<Invader>().name == "Scrap Crusher")
        {
            StatsHandler.instance.addCurrentScoreWithFeedback(score, "Scrap collected by Scrap Crusher", transform.position);
            StatsHandler.instance.addCurrentCollectedResources((uint)(scrapGain * StageHandler.instance.currentStage), true, true, transform.position);

            MyUtilities.instantiateAndDestroy(destroyEffectPrefab, transform.position, 6);

            destroyScrap(true);
        }
    }

}
