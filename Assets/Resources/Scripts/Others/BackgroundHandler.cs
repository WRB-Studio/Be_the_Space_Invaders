using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    public float spawnChance;
    public Vector2 minMaxScale;
    public Vector2 minMaxSpeed;
    private float currentSpeed;

    public Sprite atmosphereSprite;

    private Sprite[] backgroundSprites;
    private GameObject instantiatedBackgroundPrefab;

    public static BackgroundHandler instance;



    private void Awake()
    {
        instance = this;
        backgroundSprites = Resources.LoadAll<Sprite>("Sprite/Backgrounds/Planets/ByType");
    }

    private void FixedUpdate()
    {
        if (!IngameInvaderHandler.instance.allInvadersLoaded || IngameHandler.instance.isPauseOrGameOver() || TerranStationHandler.instance.stationAttackInProgress)
            return;

        if (instantiatedBackgroundPrefab != null)
        {
            Vector3 updatePosition = instantiatedBackgroundPrefab.transform.position;
            updatePosition.y += currentSpeed * Time.deltaTime;
            instantiatedBackgroundPrefab.transform.position = updatePosition;
        }
        else
        {
            spawnBackgroundPrefab();
        }
    }


    private void spawnBackgroundPrefab()
    {
        if (Random.value > spawnChance)
            return;

        GameObject newBackgroundPrefab = new GameObject("RandomBackgroundPrefab");
        newBackgroundPrefab.transform.parent = transform;

        SpriteRenderer spriteRenderer = newBackgroundPrefab.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = backgroundSprites[Random.Range(0, backgroundSprites.Length)];
        spriteRenderer.sortingOrder = -1;
        Color tmpColor = spriteRenderer.color;
        tmpColor.a = Random.Range(0.4f, 0.8f);
        spriteRenderer.color = tmpColor;

        Vector3 tmpVector3 = MyUtilities.getRandomPositionOnDisplay();
        tmpVector3.y = -MyUtilities.getDisplayDimension(Camera.main).y - 10;
        newBackgroundPrefab.transform.position = tmpVector3;

        tmpVector3 = newBackgroundPrefab.transform.rotation.eulerAngles;
        tmpVector3.z = Random.Range(0, 359);
        newBackgroundPrefab.transform.rotation = Quaternion.Euler(tmpVector3);

        float randomScale = 2;//Random.Range(minMaxScale.x, minMaxScale.y);
        newBackgroundPrefab.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        currentSpeed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);

        newBackgroundPrefab.AddComponent<CheckVisibility>();


        GameObject newAtmosphere = new GameObject("atmosphere");
        spriteRenderer = newAtmosphere.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = atmosphereSprite;
        newAtmosphere.transform.parent = newBackgroundPrefab.transform;
        newAtmosphere.transform.localPosition = Vector3.zero;
        newAtmosphere.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        tmpColor = Random.ColorHSV();
        tmpColor.a = 0.1f;
        spriteRenderer.color = tmpColor;

        instantiatedBackgroundPrefab = newBackgroundPrefab;
    }

    public void removeBackgroundPrefab()
    {
        Destroy(instantiatedBackgroundPrefab);
    }

}


/*!!!OnBecame(In)visible is also triggered by editScene camera!!!*/
public class CheckVisibility : MonoBehaviour
{
    private bool firstViewForCam = false;

    private void OnBecameVisible()
    {
        firstViewForCam = true;
    }

    private void OnBecameInvisible()
    {
        if (firstViewForCam)
            BackgroundHandler.instance.removeBackgroundPrefab();
    }
}
