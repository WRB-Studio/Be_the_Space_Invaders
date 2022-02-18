using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyUtilities : MonoBehaviour
{
    private static MyUtilities Instance;
    public static MyUtilities instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject newinstance = new GameObject("MyUtilities");
                Instance = newinstance.AddComponent<MyUtilities>();
                newinstance.transform.parent = createAndSetToScriptFolder(false).transform;
            }

            return Instance;
        }
    }

    public delegate void DelegateMethode_1();
    private static DelegateMethode_1 delegateMethode_1;


    public static GameObject createAndSetToScriptFolder(bool dontDestroyOnLoad)
    {
        string scriptfolderType = "Scripts";
        if (dontDestroyOnLoad)
            scriptfolderType = "Scripts(DontDestroyOnLoad)";

        GameObject scriptsFolder;
        if (GameObject.Find(scriptfolderType) == null)
            scriptsFolder = new GameObject(scriptfolderType);
        else
            scriptsFolder = GameObject.Find(scriptfolderType);

        return scriptsFolder;
    }


    #region Fill image with color
    public static void fillImageWithSolidColor(Image imageRenderer, Color colorVal)
    {
        Shader shaderGUItext = Shader.Find("GUI/Text Shader");
        imageRenderer.material.shader = shaderGUItext;
        imageRenderer.color = colorVal;
    }

    public static void fillImageWithShaderAndColor(Image imageRenderer, Color colorVal, Shader shaderVal)
    {
        imageRenderer.material.shader = shaderVal;
        imageRenderer.color = colorVal;
    }
    #endregion


    #region UI Anchor position
    public enum UIAnchorPosition
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public static void setAnchorPosition(ref RectTransform rectTransform, UIAnchorPosition anchorPosition)
    {
        switch (anchorPosition)
        {
            case UIAnchorPosition.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                break;
            case UIAnchorPosition.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                break;
            case UIAnchorPosition.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);
                break;
            case UIAnchorPosition.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case UIAnchorPosition.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
            case UIAnchorPosition.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case UIAnchorPosition.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                break;
            case UIAnchorPosition.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                break;
            case UIAnchorPosition.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(1, 0);
                break;
            default:
                break;
        }
    }
    #endregion


    #region List methods
    public static List<GameObject> addListToList(List<GameObject> originList, List<GameObject> extendedList)
    {
        if (originList == null || extendedList == null)
            return originList;

        for (int extendedListIndex = 0; extendedListIndex < extendedList.Count; extendedListIndex++)
            originList.Add(extendedList[extendedListIndex]);

        return originList;
    }

    public static void removeGameObjectFromList(ref List<GameObject> originList, GameObject removingObject)
    {
        originList.Remove(removingObject);
    }

    public static void removeAndDestroyGameObjectFromList(ref List<GameObject> originList, GameObject removingObject)
    {
        originList.Remove(removingObject);
        Destroy(removingObject);
    }

    public static void removeAndDestroyGameObjectFromListAfterTime(ref List<GameObject> originList, GameObject removingObject, float destroyDelay)
    {
        originList.Remove(removingObject);
        Destroy(removingObject, destroyDelay);
    }

    #endregion


    #region position methods

    public static Vector3 getMouseWorldPosition()
    {
        Vector3 mouseWorldPositon = Input.mousePosition;
        mouseWorldPositon.z = 10;
        mouseWorldPositon = Camera.main.ScreenToWorldPoint(mouseWorldPositon);
        return mouseWorldPositon;
    }

    public static Vector3 getMouseScreenPosition()
    {
        return Input.mousePosition;
    }

    #endregion


    #region Math operation methods
    public static bool isMultipleOf(int x, int n)
    {
        return (x % n) == 0;
    }

    #endregion


    #region Destroy methods

    public static GameObject instantiateAndDestroy(GameObject instantiatingObject, Vector3 spawnPosition, float lifeTime, Transform setParent = null)
    {
        GameObject newObject = Instantiate(instantiatingObject, spawnPosition, instantiatingObject.transform.rotation);

        if (setParent != null)
            newObject.transform.parent = setParent;

        Destroy(newObject, lifeTime);

        return newObject;
    }

    public static void destroyParticleSystem(GameObject destroyingObject)
    {
        destroyingObject.transform.parent = null;

        ParticleSystem particleSystem = destroyingObject.GetComponent<ParticleSystem>();

        var main = particleSystem.main;
        if(particleSystem != null)
            main.loop = false;

        Destroy(destroyingObject, main.startLifetime.constantMax + 5);
    }

    #endregion



    #region count animation for text

    public bool textCounterAnimationInProgress { get; private set; }

    private List<Coroutine> textCountAnimationCoroutines = new List<Coroutine>();

    public void textCountAnimation(uint number, float animationDuration, Text textComponent, bool playCountSound, string preText = "")
    {
        instance.textCountAnimationCoroutines.Add(instance.StartCoroutine(textCountAnimationCoroutine(number, animationDuration, textComponent, playCountSound, preText)));
    }

    private IEnumerator textCountAnimationCoroutine(uint number, float animationDuration, Text textComponent, bool playCountSound, string preText)
    {
        textCounterAnimationInProgress = true;
        yield return null;

        string oldText = "";

        if (number >= animationDuration)
        {
            uint start = 0;
            for (float timer = 0; timer < animationDuration; timer += Time.deltaTime)
            {
                float progress = timer / animationDuration;
                textComponent.text = preText + ((uint)Mathf.Lerp(start, number, progress)).ToString();
                if (playCountSound && oldText != textComponent.text)
                {
                    StaticAudioHandler.playSound(SoundChooser.instance.scoreCount);
                    oldText = textComponent.text;
                }
                yield return null;
            }
        }

        textComponent.text = preText + number.ToString();
        textCounterAnimationInProgress = false;
    }


    public void textAddCountAnimation(uint startNumber, uint addNumber, float animationDuration, Text textComponent, bool playCountSound, string preText = "")
    {
        instance.textCountAnimationCoroutines.Add(instance.StartCoroutine(textAddCountAnimationCoroutine(startNumber, addNumber, animationDuration, textComponent, playCountSound, preText)));
    }

    private IEnumerator textAddCountAnimationCoroutine(uint startNumber, uint addNumber, float animationDuration, Text textComponent, bool playCountSound, string preText)
    {
        textCounterAnimationInProgress = true;
        yield return null;

        string oldText = "";
        if ((startNumber + addNumber) >= animationDuration)
        {
            uint start = startNumber;
            for (float timer = 0; timer < animationDuration; timer += Time.deltaTime)
            {
                float progress = timer / animationDuration;
                textComponent.text = preText + ((uint)Mathf.Lerp(start, (startNumber + addNumber), progress)).ToString();
                if (playCountSound && oldText != textComponent.text)
                {
                    StaticAudioHandler.playSound(SoundChooser.instance.scoreCount);
                    oldText = textComponent.text;
                }
                yield return null;
            }
        }

        textComponent.text = preText + (startNumber + addNumber).ToString();
        textCounterAnimationInProgress = false;
    }


    public void textSubCountAnimation(uint startNumber, uint subNumber, float animationDuration, Text textComponent, bool playCountSound, string preText = "")
    {
        instance.textCountAnimationCoroutines.Add(instance.StartCoroutine(textSubCountAnimationCoroutine(startNumber, subNumber, animationDuration, textComponent, playCountSound, preText)));
    }

    private IEnumerator textSubCountAnimationCoroutine(uint startNumber, uint subNumber, float animationDuration, Text textComponent, bool playCountSound, string preText)
    {
        textCounterAnimationInProgress = true;
        yield return null;

        string oldText = "";
        if ((startNumber - subNumber) >= animationDuration)
        {
            uint start = startNumber;
            for (float timer = 0; timer < animationDuration; timer += Time.deltaTime)
            {
                float progress = timer / animationDuration;
                textComponent.text = preText + ((uint)Mathf.Lerp(start, (startNumber - subNumber), progress)).ToString();
                if (playCountSound && oldText != textComponent.text)
                {
                    StaticAudioHandler.playSound(SoundChooser.instance.scoreCount);
                    oldText = textComponent.text;
                }
                yield return null;
            }
        }

        textComponent.text = preText + (startNumber - subNumber).ToString();
        textCounterAnimationInProgress = false;
    }


    public void stopTextCountAnimation()
    {
        for (int i = 0; i < instance.textCountAnimationCoroutines.Count; i++)
        {
            instance.StopCoroutine(instance.textCountAnimationCoroutines[i]);
        }
    }

    #endregion


    #region moving and rotating methods

    public static void moveToTargetByFacing(Transform movingTransform, Transform targetTransform, float moveSpeed)
    {
        faceToTarget(movingTransform, targetTransform);

        movingTransform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
    }

    public static void moveToPositionByFacing(Transform movingTransform, Vector3 targetPosition, float moveSpeed)
    {
        faceToPosition(movingTransform, targetPosition);

        movingTransform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
    }

    public static void rotateAroundTarget(Transform movingTransform, Transform targetTransform, float speed, bool rotateFaceToTarget)
    {
        if (rotateFaceToTarget)
            faceToTarget(movingTransform, targetTransform);

        movingTransform.RotateAround(targetTransform.position, new Vector3(0, 0, 1), speed * Time.fixedDeltaTime);
    }

    public static void rotateAroundPosition(Transform movingTransform, Vector3 targetPosition, float speed, bool rotateFaceToTarget)
    {
        if (rotateFaceToTarget)
            faceToPosition(movingTransform, targetPosition);

        movingTransform.RotateAround(targetPosition, new Vector3(0, 0, 1), speed * Time.fixedDeltaTime);
    }

    public static void faceToTarget(Transform rotatingTransform, Transform targetTransform)
    {
        Vector3 diff = targetTransform.position - rotatingTransform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rotatingTransform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    public static void faceToPosition(Transform rotatingTransform, Vector3 targetPosition)
    {
        Vector3 diff = targetPosition - rotatingTransform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rotatingTransform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    #endregion


    public static void changeImageColor(GameObject imageObject, Color newColor)
    {
        imageObject.GetComponent<Image>().color = newColor;
    }

    public static void CalculateAndSetSortingOrder(List<SpriteRenderer> spriteList, SpriteRenderer spriteRenderer, int[] sortOrderRange)
    {
        bool[] freeSortOrders = new bool[sortOrderRange[1]];

        for (int i = 0; i < spriteList.Count; i++)
        {
            int currentShipSortOrder = spriteList[i].sortingOrder;

            if (currentShipSortOrder >= sortOrderRange[0] && currentShipSortOrder < sortOrderRange[1])
                freeSortOrders[currentShipSortOrder] = true;
        }

        for (int i = sortOrderRange[0]; i < freeSortOrders.Length; i++)
        {
            if (!freeSortOrders[i])
                spriteRenderer.sortingOrder = i;
        }
    }



    public static Vector3 getDisplayDimension(Camera camera)
    {
        Vector3 displayDimension = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        return displayDimension;
    }

    public static Bounds getOrthographicCameraBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public static bool objectOutOfCameraBounds(Camera camera, float extraDistance, GameObject checkingObject)
    {
        Vector3 objectPosition = checkingObject.transform.position;
        Vector3 cameraBounds = getOrthographicCameraBounds(camera).extents;

        if (objectPosition.x < -cameraBounds.x - extraDistance ||
            objectPosition.x > cameraBounds.x + extraDistance ||
            objectPosition.y < -cameraBounds.y - extraDistance ||
            objectPosition.y > cameraBounds.y + extraDistance)
            return true;

        return false;
    }

    public static Vector3 getRandomPositionOnDisplay()
    {
        Vector3 randomDisplayPosition = Vector3.zero;

        for (int i = 0; i < 10; i++)
        {
            float spawnY = Random.Range (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float spawnX = Random.Range (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

            randomDisplayPosition = new Vector2(spawnX, spawnY);
        }

        return randomDisplayPosition;
    }


    public static GameObject getNearest(List<GameObject> list, Vector3 sourcePoint)
    {
        if (list == null || list.Count <= 0)
            return null;

        GameObject nearestObject = null;
        float nearestObjectDistance = Mathf.Infinity;

        for (int listIndex = 0; listIndex < list.Count; listIndex++)
        {
            if (list[listIndex] == null)
                continue;

            GameObject listItem = list[listIndex];

            float nextObjectDistance = Vector3.Distance(sourcePoint, listItem.transform.position);
            if (nextObjectDistance < nearestObjectDistance)
            {
                nearestObject = listItem;
                nearestObjectDistance = nextObjectDistance;
            }
        }

        return nearestObject;
    }


    public static IList shuffleList(IList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }
}
