using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public enum PopupAlignment
    {
        Top, Center, Bottom
    }

    private static PopupHandler Instance;
    public static PopupHandler instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject newinstance = new GameObject("PopupHandler");
                Instance = newinstance.AddComponent<PopupHandler>();
                newinstance.transform.parent = MyUtilities.createAndSetToScriptFolder(false).transform;

                Instance.loadPopupPrefabs();
            }

            return Instance;
        }

        set
        {
            if (Instance == null)
                Instance = value;
        }
    }

    public delegate void OnBt1Methode();
    public delegate void OnBt2Methode();
    private static OnBt1Methode onBt1Call;
    private static OnBt2Methode onBt2Call;

    private static GameObject popupWithButtonPrefab;
    private static GameObject instantiatedPopupWithButtons;

    private static GameObject shortPopupInfoPrefab;
    private static GameObject instantiatedShortInfoPopup;
    private static float shortPopupInfoFadeSpeed = 0.03f;
    private static float shortPopupInfoShowDuration = 1.5f;

    private static GameObject headerOneButtonPopupPrefab;
    private static GameObject instantiatedHeaderOneButtonPopup;

    private static GameObject headerSidePopupPrefab;
    private static GameObject instantiatedHeaderSidePopup;

    private static GameObject invaderInfoPopupPrefab;
    private static GameObject instantiatedInvaderInfoPopup;

    private static GameObject reportBugPopupPrefab;
    private static GameObject instantiatedreportBugPopupPrefab;



    private void loadPopupPrefabs()
    {
        popupWithButtonPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasPopupWithButtons");
        shortPopupInfoPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasShortPopupInfo");
        headerOneButtonPopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasHeaderOneButtonPopup");
        headerSidePopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasSidePopup");
        invaderInfoPopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasInvaderInfoPopup");
        reportBugPopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popups/CanvasPopupBugReport");
    }

    private void setPanelAlignment(Transform panelTransform, PopupAlignment alignment)
    {
        RectTransform rectTransform = panelTransform.GetComponent<RectTransform>();

        switch (alignment)
        {
            case PopupAlignment.Top:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                panelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
                break;
            case PopupAlignment.Center:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                panelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                break;
            case PopupAlignment.Bottom:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                panelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);
                break;
            default:
                break;
        }
    }

    private bool popupWithSameInfoExists(string newInfo, string oldInfo, GameObject popupGO)
    {
        if (popupGO != null && newInfo == oldInfo)
            return true;

        return false;
    }

    private void playPopupSound()
    {
        StaticAudioHandler.playSound(SoundChooser.instance.openPopup);
    }



    /*-------------------------------Area: Popup with buttons---------------------------------------*/

    public void showPopupWithOKButton(string info)
    {
        if (instantiatedPopupWithButtons != null && popupWithSameInfoExists(info, instantiatedPopupWithButtons.transform.GetChild(1).GetChild(0).GetComponent<Text>().text, instantiatedPopupWithButtons))
            return;

        StartCoroutine(showPopupWithOneButtonCoroutine(info, "OK", null));
    }

    public void showPopupWithOneButton(string info, string button1Text, OnBt1Methode newOnBt1Methode)
    {
        if (instantiatedPopupWithButtons != null && popupWithSameInfoExists(info, instantiatedPopupWithButtons.transform.GetChild(1).GetChild(0).GetComponent<Text>().text, instantiatedPopupWithButtons))
            return;

        StartCoroutine(showPopupWithOneButtonCoroutine(info, button1Text, newOnBt1Methode));
    }

    private IEnumerator showPopupWithOneButtonCoroutine(string info, string button1Text, OnBt1Methode newOnBt1Methode)
    {
        //Waiting for the end of the current information
        while (instantiatedPopupWithButtons != null && instantiatedPopupWithButtons.activeSelf)
            yield return new WaitForSeconds(0.5f);

        instantiatedPopupWithButtons = Instantiate(popupWithButtonPrefab);
        playPopupSound();

        //show popup info
        setInfoText(info);
        setButton1(button1Text, newOnBt1Methode);
        hideButton2();
    }


    public void showPopupWithTwoButton(string info, string button1Text, OnBt1Methode newOnBt1Methode, string button2Text, OnBt2Methode newOnBt2Methode)
    {
        if (instantiatedPopupWithButtons != null && popupWithSameInfoExists(info, instantiatedPopupWithButtons.transform.GetChild(1).GetChild(0).GetComponent<Text>().text, instantiatedPopupWithButtons))
            return;

        StartCoroutine(showPopupWithTwoButtonCoroutine(info, button1Text, newOnBt1Methode, button2Text, newOnBt2Methode));
    }

    private IEnumerator showPopupWithTwoButtonCoroutine(string info, string button1Text, OnBt1Methode newOnBt1Methode, string button2Text, OnBt2Methode newOnBt2Methode)
    {
        //Waiting for the end of the current information
        while (instantiatedPopupWithButtons != null && instantiatedPopupWithButtons.activeSelf)
            yield return new WaitForSeconds(0.5f);
        instantiatedPopupWithButtons = Instantiate(popupWithButtonPrefab);
        playPopupSound();

        //show popup info
        setInfoText(info);
        setButton1(button1Text, newOnBt1Methode);
        setButton2(button2Text, newOnBt2Methode);
    }


    private void setInfoText(string info)
    {
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = info;
    }

    private void setButton1(string button1Info, OnBt1Methode newOnBt1Methode)
    {
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = button1Info;
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (newOnBt1Methode != null)
                newOnBt1Methode();

            Destroy(instantiatedPopupWithButtons);
        });
    }

    private void setButton2(string button2Info, OnBt2Methode newOnBt2Methode)
    {
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = button2Info;
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (newOnBt2Methode != null)
                newOnBt2Methode();

            Destroy(instantiatedPopupWithButtons);
        });
    }

    private void hideButton1()
    {
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(false);
    }

    private void hideButton2()
    {
        instantiatedPopupWithButtons.transform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
    }



    /*-------------------------------Area: Short info Popups---------------------------------------*/
    private Coroutine activeCoroutine;

    public void showShortInfoPopup(string info, PopupAlignment alignment = PopupAlignment.Center)
    {
        if (instantiatedShortInfoPopup != null && popupWithSameInfoExists(info, instantiatedShortInfoPopup.transform.GetChild(0).GetComponent<Text>().text, instantiatedShortInfoPopup))
            return;

        StartCoroutine(showShortInfoPopupCoroutine(info, alignment));
    }

    private IEnumerator showShortInfoPopupCoroutine(string info, PopupAlignment alignment)
    {
        //Waiting for the end of the current information
        while (instantiatedShortInfoPopup != null && instantiatedShortInfoPopup.activeSelf)
            yield return new WaitForSeconds(0.5f);

        instantiatedShortInfoPopup = Instantiate(shortPopupInfoPrefab);

        setPanelAlignment(instantiatedShortInfoPopup.transform.GetChild(0), alignment);

        bool speedUpFading = false;

        //show short popup info
        Text txtShortInfo = instantiatedShortInfoPopup.transform.GetChild(0).GetComponent<Text>();
        txtShortInfo.text = info;

        //text fade in -> wait -> fade out
        Color tmpTextColor = txtShortInfo.color;
        tmpTextColor.a = 0;
        txtShortInfo.color = tmpTextColor;

        while (txtShortInfo.color.a < 1)
        {
            yield return new WaitForSeconds(shortPopupInfoFadeSpeed);

            tmpTextColor = txtShortInfo.color;
            tmpTextColor.a += shortPopupInfoFadeSpeed;
            if (speedUpFading)
                tmpTextColor.a += shortPopupInfoFadeSpeed * 2;

            if (tmpTextColor.a > 1)
                tmpTextColor.a = 1;

            txtShortInfo.color = tmpTextColor;

            if (Input.GetMouseButtonDown(0))
                speedUpFading = true;
        }

        float timeCounter = shortPopupInfoShowDuration;
        while (timeCounter > 0)
        {
            timeCounter -= Time.fixedDeltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                speedUpFading = true;
                break;
            }
        }

        while (txtShortInfo.color.a > 0)
        {
            yield return new WaitForSeconds(shortPopupInfoFadeSpeed);

            tmpTextColor = txtShortInfo.color;
            tmpTextColor.a -= shortPopupInfoFadeSpeed;
            if (speedUpFading)
                tmpTextColor.a -= shortPopupInfoFadeSpeed * 2;

            if (tmpTextColor.a < 0)
                tmpTextColor.a = 0;

            txtShortInfo.color = tmpTextColor;

            if (Input.GetMouseButtonDown(0))
                speedUpFading = true;
        }

        Destroy(instantiatedShortInfoPopup);
    }

    public void showShortInfoPopupRefreshCurrentWhenMultiple(string info, PopupAlignment alignment = PopupAlignment.Center)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(showShortInfoPopupRefreshCurrentWhenMultipleCoroutine(info, alignment));
    }

    private IEnumerator showShortInfoPopupRefreshCurrentWhenMultipleCoroutine(string info, PopupAlignment alignment)
    {
        bool refreshPopup = true;

        if (instantiatedShortInfoPopup == null)
        {
            instantiatedShortInfoPopup = Instantiate(shortPopupInfoPrefab);
            refreshPopup = false;
        }

        setPanelAlignment(instantiatedShortInfoPopup.transform.GetChild(0), alignment);

        bool speedUpFading = false;

        //show short popup info
        Text txtShortInfo = instantiatedShortInfoPopup.transform.GetChild(0).GetComponent<Text>();
        txtShortInfo.text = info;

        //text fade in -> wait -> fade out
        Color tmpTextColor = txtShortInfo.color;
        if (!refreshPopup)
            tmpTextColor.a = 0;
        txtShortInfo.color = tmpTextColor;

        while (txtShortInfo.color.a < 1)
        {
            yield return new WaitForSeconds(shortPopupInfoFadeSpeed);

            tmpTextColor = txtShortInfo.color;
            tmpTextColor.a += shortPopupInfoFadeSpeed;
            if (speedUpFading)
                tmpTextColor.a += shortPopupInfoFadeSpeed * 2;

            if (tmpTextColor.a > 1)
                tmpTextColor.a = 1;

            txtShortInfo.color = tmpTextColor;

            if (Input.GetMouseButtonDown(0))
                speedUpFading = true;
        }

        float timeCounter = shortPopupInfoShowDuration;
        while (timeCounter > 0)
        {
            timeCounter -= Time.fixedDeltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                speedUpFading = true;
                break;
            }
        }

        while (txtShortInfo.color.a > 0)
        {
            yield return new WaitForSeconds(shortPopupInfoFadeSpeed);

            tmpTextColor = txtShortInfo.color;
            tmpTextColor.a -= shortPopupInfoFadeSpeed;
            if (speedUpFading)
                tmpTextColor.a -= shortPopupInfoFadeSpeed * 2;

            if (tmpTextColor.a < 0)
                tmpTextColor.a = 0;

            txtShortInfo.color = tmpTextColor;

            if (Input.GetMouseButtonDown(0))
                speedUpFading = true;
        }

        Destroy(instantiatedShortInfoPopup);
        activeCoroutine = null;
    }


    /*-------------------------------Area: Popup with Header & one button---------------------------------------*/

    public void showHeaderOneButtonPopup(string headerInfo, string info, string buttonText = "OK", OnBt1Methode newOnBtMethode = null)
    {
        if (instantiatedHeaderOneButtonPopup != null && popupWithSameInfoExists(info, instantiatedHeaderOneButtonPopup.transform.GetChild(1).GetChild(1).GetComponent<Text>().text, headerOneButtonPopupPrefab))
            return;

        StartCoroutine(showHeaderOneButtonPopupCoroutine(headerInfo, info, buttonText, newOnBtMethode));
    }

    private IEnumerator showHeaderOneButtonPopupCoroutine(string headerInfo, string info, string buttonText, OnBt1Methode newOnBt1Methode)
    {
        //Waiting for the end of the current information
        while (instantiatedHeaderOneButtonPopup != null && instantiatedHeaderOneButtonPopup.activeSelf)
            yield return new WaitForSeconds(0.5f);

        instantiatedHeaderOneButtonPopup = Instantiate(headerOneButtonPopupPrefab);
        playPopupSound();

        //header info
        instantiatedHeaderOneButtonPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = headerInfo;

        //info text
        instantiatedHeaderOneButtonPopup.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = info;

        //button text
        instantiatedHeaderOneButtonPopup.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>().text = buttonText;

        instantiatedHeaderOneButtonPopup.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (newOnBt1Methode != null)
                newOnBt1Methode();

            Destroy(instantiatedHeaderOneButtonPopup);
        });
    }



    /*-------------------------------Area: Popup for invader infos---------------------------------*/

    public void showInvaderInfoPopup(Invader invaderScrp, PopupAlignment alignment = PopupAlignment.Bottom)
    {
        if (instantiatedInvaderInfoPopup != null)
            Destroy(instantiatedInvaderInfoPopup);

        instantiatedInvaderInfoPopup = Instantiate(invaderInfoPopupPrefab);
        playPopupSound();

        setPanelAlignment(instantiatedInvaderInfoPopup.transform.GetChild(1), alignment);

        //set popup infos
        instantiatedInvaderInfoPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = invaderScrp.name;
        instantiatedInvaderInfoPopup.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = invaderScrp.invaderInfo;


        Transform invaderPopupInfoItemParent = instantiatedInvaderInfoPopup.transform.GetChild(1).GetChild(2);
        GameObject invaderPopupInfoItemPrefab = invaderPopupInfoItemParent.GetChild(0).gameObject;

        List<string[]> invaderStatInfos = getInvaderStatsInfos(invaderScrp);
        List<string[]> invaderWeaponInfos = getInvaderWeaponInfos(invaderScrp);

        int infosCount = invaderStatInfos.Count;
        if (invaderWeaponInfos.Count > invaderStatInfos.Count)
            infosCount = invaderWeaponInfos.Count;

        for (int i = 0; i < infosCount; i++)
        {
            GameObject newInvaderPopupInfoItem = Instantiate(invaderPopupInfoItemPrefab, invaderPopupInfoItemParent);

            string shipStatName = null;
            string shipStatValue = null;
            string shipStatLVLUPValue = null;
            string weaponStatName = null;
            string weaponStatValue = null;
            string weaponStatLVLUPValue = null;

            if (i < invaderStatInfos.Count && invaderStatInfos[i] != null)
            {
                shipStatName = invaderStatInfos[i][0];
                shipStatValue = invaderStatInfos[i][1];
                shipStatLVLUPValue = invaderStatInfos[i][2];
            }

            if (i < invaderWeaponInfos.Count && invaderWeaponInfos[i] != null)
            {
                weaponStatName = invaderWeaponInfos[i][0];
                weaponStatValue = invaderWeaponInfos[i][1];
                weaponStatLVLUPValue = invaderWeaponInfos[i][2];

                if (i > 0 && invaderWeaponInfos[i][1] == null)
                    weaponStatName = "\n" + weaponStatName;
            }

            newInvaderPopupInfoItem.GetComponent<InvaderPopupInfoItem>().init(shipStatName, shipStatValue, shipStatLVLUPValue, weaponStatName, weaponStatValue, weaponStatLVLUPValue);
        }

        Destroy(invaderPopupInfoItemParent.transform.GetChild(0).gameObject);


        instantiatedInvaderInfoPopup.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(instantiatedInvaderInfoPopup);
        });
    }

    private List<string[]> getInvaderStatsInfos(Invader invaderScrp)
    {
        List<string[]> invaderStatInfos = new List<string[]>();

        string[] stringArrayInfos = new string[3];
        stringArrayInfos[0] = "Level:";
        stringArrayInfos[1] = invaderScrp.level.ToString();
        stringArrayInfos[2] = " >> " + invaderScrp.getNextLevel();
        invaderStatInfos.Add(stringArrayInfos);

        stringArrayInfos = new string[3];
        stringArrayInfos[0] = "Hitpoints:";
        stringArrayInfos[1] = invaderScrp.currentHitpoints.ToString();
        stringArrayInfos[2] = " >> " + invaderScrp.getHitpointsOfLevelN(invaderScrp.level + 1);
        invaderStatInfos.Add(stringArrayInfos);

        return invaderStatInfos;
    }

    private List<string[]> getInvaderWeaponInfos(Invader invaderScrp)
    {
        List<string[]> weaponInfoList = new List<string[]>();
        ActiveWeapon[] activeWeaponList = null;

        activeWeaponList = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Laser);
        if (activeWeaponList != null)
            weaponInfoList = getWeaponInfos(invaderScrp, activeWeaponList, weaponInfoList);
        activeWeaponList = null;

        activeWeaponList = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Bomb);
        if (activeWeaponList != null)
            weaponInfoList = getWeaponInfos(invaderScrp, activeWeaponList, weaponInfoList);
        activeWeaponList = null;

        activeWeaponList = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Missle_launcher);
        if (activeWeaponList != null)
            weaponInfoList = getWeaponInfos(invaderScrp, activeWeaponList, weaponInfoList);
        activeWeaponList = null;

        if (invaderScrp.getShield() != null)
        {
            activeWeaponList = new ActiveWeapon[] { invaderScrp.getShield() };
            if (activeWeaponList != null)
                weaponInfoList = getShieldInfos(invaderScrp, activeWeaponList, weaponInfoList);
            activeWeaponList = null;
        }

        if (invaderScrp.getSupportSystem() != null)
        {
            activeWeaponList = new ActiveWeapon[] { invaderScrp.getSupportSystem() };
            if (activeWeaponList != null)
                weaponInfoList = getSupportSystemInfos(invaderScrp, activeWeaponList, weaponInfoList);
            activeWeaponList = null;
        }

        return weaponInfoList;
    }

    private List<string[]> getWeaponInfos(Invader invaderScrp, ActiveWeapon[] activeWeaponList, List<string[]> weaponInfoList)
    {
        string[] weaponInfo = new string[3];

        if (activeWeaponList != null)
        {
            if (activeWeaponList.Length == 1)
                weaponInfo[0] = activeWeaponList[0].weaponType.ToString().Replace("_", " ");
            else
                weaponInfo[0] = activeWeaponList.Length + "x " + activeWeaponList[0].weaponType.ToString().Replace("_", " ");
            weaponInfoList.Add(weaponInfo);

            weaponInfo = new string[3];
            ActiveWeapon activeWeapon = activeWeaponList[0];
            weaponInfo[0] = "\tCooldown: ";
            weaponInfo[1] = activeWeapon.cooldown + " s";
            weaponInfo[2] = " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfoList.Add(weaponInfo);

            weaponInfo = new string[3];
            weaponInfo[0] = "\tSpeed: ";
            weaponInfo[1] = activeWeapon.speed * 100 + " m/s";
            weaponInfo[2] = " >> " + activeWeapon.getSpeedOfLevelN(invaderScrp.level + 1) * 100 + " m/s";
            weaponInfoList.Add(weaponInfo);

            weaponInfo = new string[3];
            weaponInfo[0] = "\tDamage: ";
            weaponInfo[1] = activeWeapon.damage.ToString();
            weaponInfo[2] = " >> " + activeWeapon.getDamageOfLevelN(invaderScrp.level + 1);
            weaponInfoList.Add(weaponInfo);
        }

        return weaponInfoList;
    }

    private List<string[]> getShieldInfos(Invader invaderScrp, ActiveWeapon[] activeShieldList, List<string[]> weaponInfoList)
    {
        string[] shieldInfo = new string[3];

        if (activeShieldList != null)
        {
            shieldInfo[0] = activeShieldList[0].weaponType.ToString().Replace("_", " ");
            weaponInfoList.Add(shieldInfo);

            shieldInfo = new string[3];
            ActiveWeapon activeWeapon = activeShieldList[0];
            shieldInfo[0] = "\t\tCooldown: ";
            shieldInfo[1] = activeWeapon.cooldown + " s";
            shieldInfo[2] = " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfoList.Add(shieldInfo);

            shieldInfo = new string[3];
            shieldInfo[0] = "\t\tHitpoints: ";
            shieldInfo[1] = activeWeapon.shieldHitpoints.ToString();
            shieldInfo[2] = " >> " + activeWeapon.getShieldHitpointsOfLevelN(invaderScrp.level + 1).ToString();
            weaponInfoList.Add(shieldInfo);

            shieldInfo = new string[3];
            shieldInfo[0] = "\t\tLife time: ";
            shieldInfo[1] = Math.Round(activeWeapon.shieldTime, 2) + " s";
            shieldInfo[2] = " >> " + Math.Round(activeWeapon.getShieldTimeOfLevelN(invaderScrp.level + 1), 2) + " s";
            weaponInfoList.Add(shieldInfo);
        }

        return weaponInfoList;
    }

    private List<string[]> getSupportSystemInfos(Invader invaderScrp, ActiveWeapon[] activeSupportSystemList, List<string[]> weaponInfoList)
    {
        string[] supportSystemInfo = new string[3];

        if (activeSupportSystemList != null)
        {
            supportSystemInfo[0] = activeSupportSystemList[0].weaponType.ToString().Replace("_", " ");
            weaponInfoList.Add(supportSystemInfo);

            supportSystemInfo = new string[3];
            ActiveWeapon activeWeapon = activeSupportSystemList[0];
            supportSystemInfo[0] = "\t\tCooldown: ";
            supportSystemInfo[1] = activeWeapon.cooldown + " s";
            supportSystemInfo[2] = " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfoList.Add(supportSystemInfo);

            supportSystemInfo = new string[3];
            supportSystemInfo[0] = "\t\tDuration: ";
            supportSystemInfo[1] = Math.Round(activeWeapon.effectDuration, 2) + " s";
            supportSystemInfo[2] = " >> " + activeWeapon.getEffectDurationOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfoList.Add(supportSystemInfo);

            if (activeWeapon.weaponType == Weapon.WeaponType.Repair_drone || activeWeapon.weaponType == Weapon.WeaponType.Collector_drone)
            {
                supportSystemInfo = new string[3];
                supportSystemInfo[0] = "\t\tDrones: ";
                supportSystemInfo[1] = activeWeapon.dronesQuantity.ToString();
                supportSystemInfo[2] = " >> " + activeWeapon.getDronesQuantityOfLevelN(invaderScrp.level + 1).ToString();
                weaponInfoList.Add(supportSystemInfo);

                supportSystemInfo = new string[3];
                supportSystemInfo[0] = "\t\tSpeed: ";
                supportSystemInfo[1] = Math.Round(activeWeapon.speed * 100, 2) + " m/s";
                supportSystemInfo[2] = " >> " + activeWeapon.getSpeedOfLevelN(invaderScrp.level + 1) * 100 + " m/s";
                weaponInfoList.Add(supportSystemInfo);
            }

            if (activeWeapon.weaponType == Weapon.WeaponType.Collector_drone)
            {
                supportSystemInfo = new string[3];
                supportSystemInfo[0] = "\t\tRes. extraction: ";
                supportSystemInfo[1] = activeWeapon.resourceExtraction.ToString();
                supportSystemInfo[2] = " >> " + activeWeapon.getResourceCollectingExtractionOfLevelN(invaderScrp.level + 1).ToString();
                weaponInfoList.Add(supportSystemInfo);
            }
        }

        return weaponInfoList;
    }



    /*-------------------------------Area: Report Bugs Popup---------------------------------*/

    public void showReportBugPopup()
    {
        if (instantiatedreportBugPopupPrefab != null)
            return;

        StartCoroutine(showReportBugPopupCoroutine());
    }

    private IEnumerator showReportBugPopupCoroutine()
    {
        yield return null;

        instantiatedreportBugPopupPrefab = Instantiate(reportBugPopupPrefab);
        playPopupSound();

        //send button
        instantiatedreportBugPopupPrefab.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            InputField ifReportBug = instantiatedreportBugPopupPrefab.transform.GetChild(1).GetChild(1).GetComponent<InputField>();
            //send mail
            if (ifReportBug.text.Length > 15)
            {
                EmailSend.instance.sendBugReportMail(ifReportBug.text);
                Destroy(instantiatedreportBugPopupPrefab);

                showShortInfoPopup("Thank you for the report!", PopupAlignment.Top);
            }
            else
            {
                instantiatedreportBugPopupPrefab.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "Please enter a description!";
            }
        });

        //close button
        instantiatedreportBugPopupPrefab.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        { Destroy(instantiatedreportBugPopupPrefab); });
    }


}
