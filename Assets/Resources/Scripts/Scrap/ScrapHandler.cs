using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapHandler : MonoBehaviour
{
    private GameObject[] scrapPrefabs;
    private List<GameObject> instantiatedScrap;

    private List<AudioSource> resCollectingSound = new List<AudioSource>();

    [Header("Multiplier settings")]
    public Transform imgscoreMultiplier;
    public Text txtScoreMultiplier;
    private Vector3 originImgScoreScale;
    public float multiplierMaxDelay = 0.5f;
    private float currentMultiplierTime;
    private int currentMultiplier = 1;
    private uint currentMultiplierScore = 0;
    private bool multiplierCoroutineInProgress = false;
    public GameObject multiplierFeedbackPrefab;

    public static ScrapHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        scrapPrefabs = Resources.LoadAll<GameObject>("Prefabs/Resources");
        instantiatedScrap = new List<GameObject>();
        imgscoreMultiplier.gameObject.SetActive(false);
        txtScoreMultiplier.text = "";
        originImgScoreScale = imgscoreMultiplier.transform.localScale;
        currentMultiplierTime = multiplierMaxDelay;
    }

    private void FixedUpdate()
    {
        if (IngameHandler.instance.isPauseOrGameOver())
            return;

        for (int scrapIndex = 0; scrapIndex < instantiatedScrap.Count; scrapIndex++)
        {
            if (instantiatedScrap[scrapIndex] == null)
                continue;

            instantiatedScrap[scrapIndex].GetComponent<Scrap>().updateCall();
        }
    }


    public void spawnScrap(Transform sourceTransform, int amount)
    {
        float spawnExtraDistance = 0.5f;

        for (int i = 0; i < amount; i++)
        {
            GameObject newScrap = Instantiate(scrapPrefabs[Random.Range(0, scrapPrefabs.Length - 1)], transform);

            Vector3 spawnPosition = sourceTransform.position;
            newScrap.transform.position = new Vector2(Random.Range(spawnPosition.x - spawnExtraDistance, spawnPosition.x + spawnExtraDistance), Random.Range(spawnPosition.y - spawnExtraDistance, spawnPosition.y + spawnExtraDistance));

            newScrap.GetComponent<Scrap>().init(sourceTransform);

            instantiatedScrap.Add(newScrap);
        }
    }

    public List<GameObject> getInstantiatedScrap()
    {
        return instantiatedScrap;
    }


    public void addMultiplier(uint score, Vector3 position)
    {
        currentMultiplier++;
        currentMultiplierScore += score;
        currentMultiplierTime = multiplierMaxDelay;

        if (currentMultiplier > 1)
        {
            imgscoreMultiplier.gameObject.SetActive(true);
            txtScoreMultiplier.text = "x" + currentMultiplier;
        }

        if (imgscoreMultiplier.transform.localScale.x < 1.5f)
        {
            Vector3 tmpScale = imgscoreMultiplier.transform.localScale;
            tmpScale.x += (1.5f - 1f) / 40f;
            tmpScale.y += (1.5f - 1f) / 40f;
            imgscoreMultiplier.transform.localScale = tmpScale;
        }

        if (!multiplierCoroutineInProgress)
            StartCoroutine(multiplierCoroutine());

    }

    private IEnumerator multiplierCoroutine()
    {
        multiplierCoroutineInProgress = true;
        imgscoreMultiplier.transform.localScale = originImgScoreScale;


        while (currentMultiplierTime > 0)
        {
            yield return null;
            currentMultiplierTime -= Time.deltaTime;
        }

        uint tmpMultiplierScore = 0;
        int tmpCurrentMultiplier = currentMultiplier;
        uint tmpCalc = 0;
        if (currentMultiplier > 1)
        {
            Debug.Log("tmpCalc: " + currentMultiplier + " / 100 * 20 " + " = " + (currentMultiplier / 100f * 10f));

            tmpCalc = (uint)(currentMultiplier / 100f * 20f);

            if (tmpCalc < 1)
                tmpCalc = 1;
            tmpMultiplierScore = (uint)(currentMultiplier * tmpCalc * currentMultiplierScore);
        }

        Debug.Log("tmpMultiplierScore: " + currentMultiplier + " * " + tmpCalc + " * " + currentMultiplierScore + " = " + tmpMultiplierScore);

        currentMultiplier = 0;
        currentMultiplierScore = 0;
        currentMultiplierTime = multiplierMaxDelay;

        multiplierCoroutineInProgress = false;

        yield return new WaitForSeconds(0.4f);

        if (tmpMultiplierScore > 1)
        {
            GameObject newMultiplierFeedback = Instantiate(multiplierFeedbackPrefab);
            newMultiplierFeedback.transform.GetChild(0).GetComponent<Text>().text = "" + tmpMultiplierScore;
            Destroy(newMultiplierFeedback, 2f);

            StatsHandler.instance.checkForHighestScoreMultiplier(tmpCurrentMultiplier, tmpMultiplierScore);
            StartCoroutine(scoreMultiplicatorAddDelayCoroutine(tmpMultiplierScore));
        }

        if (!multiplierCoroutineInProgress)
        {
            yield return new WaitForSeconds(0.3f);

            txtScoreMultiplier.text = "";
            imgscoreMultiplier.gameObject.SetActive(false);
            imgscoreMultiplier.transform.localScale = originImgScoreScale;
        }
    }

    private IEnumerator scoreMultiplicatorAddDelayCoroutine(uint scoreVal)
    {
        yield return new WaitForSeconds(1.1f);
        StatsHandler.instance.addCurrentScore(scoreVal, "score multiplicator by collected scrap");
    }


    public void removeScrapFromList(GameObject removingObject, bool destroyCollecting)
    {
        instantiatedScrap.Remove(removingObject);
        Destroy(removingObject);

        for (int i = 0; i < resCollectingSound.ToArray().Length; i++)
        {
            if (resCollectingSound[i] == null)
                resCollectingSound.Remove(resCollectingSound[i]);
        }

        if (destroyCollecting && resCollectingSound.Count < 2)
        {
            resCollectingSound.Add(StaticAudioHandler.playSound(SoundChooser.instance.collectResources));
        }
    }
}
