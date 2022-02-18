using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidHandler : MonoBehaviour
{
    [Header("Spawn settings")]
    public bool enableSpawning = true;
    public float spawnIntervallCheck;
    private float spawnCoolDown;
    public float spawnChance;
    public int maxAsteroids;

    [Header("Resource gain calculation")]
    public int startResource = 1;
    public int divisor = 3;

    private GameObject[] asteroidPrefabs;
    private List<GameObject> instantiatedAsteroids;

    public static AsteroidHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        asteroidPrefabs = Resources.LoadAll<GameObject>("Prefabs/Asteroids").ToArray();
        instantiatedAsteroids = new List<GameObject>();

        spawnCoolDown = spawnIntervallCheck;
    }

    private void FixedUpdate()
    {
        if (IngameHandler.instance.isPauseOrGameOver())
            return;

        for (int asteroidIndex = 0; asteroidIndex < instantiatedAsteroids.Count; asteroidIndex++)
        {
            if (instantiatedAsteroids[asteroidIndex] == null)
                continue;

            instantiatedAsteroids[asteroidIndex].GetComponent<Asteroid>().updateCall();
        }


        spawnCoolDown -= Time.fixedDeltaTime;
        if (spawnCoolDown <= 0)
        {
            spawnCoolDown = spawnIntervallCheck;
            spawnAsteroid();
        }

    }


    private void spawnAsteroid()
    {
        if (!enableSpawning || Random.value > spawnChance || instantiatedAsteroids.Count >= maxAsteroids)
            return;
        
        GameObject newAsteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length - 1)], transform);
        
        Vector3 displayDimension = MyUtilities.getOrthographicCameraBounds(Camera.main).extents;
        Vector3 spawnPosition = new Vector3(Random.Range(-displayDimension.x, displayDimension.x), -displayDimension.y - 0.5f, 0);
        newAsteroid.transform.position = spawnPosition;

        Vector3 targetPosition = new Vector3(Random.Range(-displayDimension.x, displayDimension.x), displayDimension.y + 2, 0);

        int resGain = startResource * StageHandler.instance.currentStage / divisor;
        if (resGain <= 0)
            resGain = 1;
        newAsteroid.GetComponent<Asteroid>().init(targetPosition, resGain);

        instantiatedAsteroids.Add(newAsteroid);
    }

    public List<GameObject> getInstantiatedAsteroids()
    {
        return instantiatedAsteroids;
    }

    public void removeAsteroidFromList(GameObject removingObject)
    {
        MyUtilities.removeAndDestroyGameObjectFromList(ref instantiatedAsteroids, removingObject);
    }
}
