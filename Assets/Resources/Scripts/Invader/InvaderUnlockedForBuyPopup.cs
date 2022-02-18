using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvaderUnlockedForBuyPopup : MonoBehaviour
{
    public GameObject popupPrefab;

    private bool popupInProgress = false;

    public static InvaderUnlockedForBuyPopup instance;



    private void Awake()
    {
        instance = this;
    }

    public void show(Sprite invaderImage, string invaderName, string info)
    {
        StartCoroutine(showCoroutine(invaderImage, invaderName, info));
    }


    private IEnumerator showCoroutine(Sprite invaderImage, string invaderName, string info)
    {
        while (popupInProgress) { yield return null; }

        popupInProgress = true;

        yield return new WaitForSeconds(0.5f);

        GameObject newPopup = Instantiate(popupPrefab, transform);

        newPopup.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = invaderImage;
        newPopup.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = invaderName;
        newPopup.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = info;

        yield return new WaitForSeconds(3);

        Destroy(newPopup);

        popupInProgress = false;

    }

}
