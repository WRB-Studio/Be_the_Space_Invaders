using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvaderListItem : MonoBehaviour
{
    [Header("UI elements")]
    public Sprite unlockedBackground;
    public Image imgItemBackground;
    public Image imgInvader;
    public Text txtName;
    public Text txtLevel;
    public Text txtStats;
    public Text txtWeapons;
    public Text txtUnlockConditions;
    private string statsLevelFrontString = "\t\t";

    public Button btBuyUpgrade;
    public Text txtBtBuyUpgradeInfo;
    public Text txtBtBuyUpgradeCost;

    public Button btInfo;

    [Header("Color settings")]
    public Color lockedBackgroundColor;
    public Color unlockedBackgroundColor;
    public Color canBuyButtonColor;
    public Color cantBuyButtonColor;

    [Header("Other settings")]
    public float imgInvaderHoverScaleAdd = 0.2f;
    private Vector2 originImgSize;
    private Vector2 hoverImgSize;

    public Invader invaderScrp { get; private set; }



    public void init(int newID)
    {
        invaderScrp = InvaderLoader.instance.getLoadedInvaderWithID(newID).GetComponent<Invader>();

        imgInvader.sprite = invaderScrp.invaderTexture.sprite;//invader image
        Vector2 imgRect = imgInvader.rectTransform.sizeDelta;
        if (invaderScrp.formationSlotSize.neededFormationSlots == 1)
        {
            imgRect.x = imgRect.x - imgRect.x / 4;
            imgRect.y = imgRect.y - imgRect.y / 4;
            imgInvader.rectTransform.sizeDelta = imgRect;
        }
        originImgSize = imgRect;
        hoverImgSize = new Vector2(imgRect.x + imgInvaderHoverScaleAdd, imgRect.y + imgInvaderHoverScaleAdd);

        setInfos();
        onBtBuyOrUpgrade();
    }


    private void setInfos()
    {
        txtName.text = invaderScrp.name;//invader name

        //Invader level
        txtLevel.text = invaderScrp.level.ToString();

        //check unlock condition fullfilled are new
        bool tmpUnlockConditionsFullfilled = invaderScrp.getUnlockConditionsFullfilled();
        invaderScrp.checkUnlockConditionsFullfilled();
        if (!tmpUnlockConditionsFullfilled && invaderScrp.getUnlockConditionsFullfilled())
        {
            InvaderUnlockedForBuyPopup.instance.show(invaderScrp.invaderTexture.sprite, invaderScrp.name, invaderScrp.name + " is ready for purchase.");
        }

        //show infos when unlock conditions fullfilled
        if (invaderScrp.getUnlockConditionsFullfilled())
        {
            txtUnlockConditions.gameObject.SetActive(false);
            txtStats.gameObject.SetActive(true);
            txtWeapons.gameObject.SetActive(true);
            btBuyUpgrade.gameObject.SetActive(true);
            btInfo.gameObject.SetActive(true);

            //Invader stats
            string invaderStats = "";
            invaderStats += "HP: " + invaderScrp.currentHitpoints;
            invaderStats += "\nSlotsize: " + invaderScrp.formationSlotSize.column + "x" + invaderScrp.formationSlotSize.row;
            invaderStats += "\nIn formation: " + FormationHandler.instance.getSettedInvadersById(invaderScrp.id) + "/" + invaderScrp.maxInFormation;
            txtStats.GetComponent<Text>().text = invaderStats;

            //Invader weapon infos
            txtWeapons.text = "Weapons:\n" + getWeaponInfosAsString(false, "\t");
        }
        else //show only specific infos when unlock conditions not fullfilled
        {
            txtUnlockConditions.gameObject.SetActive(true);
            txtStats.gameObject.SetActive(false);
            txtWeapons.gameObject.SetActive(false);
            btBuyUpgrade.gameObject.SetActive(false);
            btInfo.gameObject.SetActive(false);

            txtUnlockConditions.text = "Unlock conditions:\n" + invaderScrp.getUnlockConditionsInfo();
        }


        setlockedUnlockedInfos();

        btInfo.onClick.RemoveAllListeners();
        btInfo.onClick.AddListener(() =>
        {
            onClickBtInfo();
        });

        addEventListeners();
    }

    private void setlockedUnlockedInfos()
    {
        if (invaderScrp.unlocked) //when unlocked
        {
            GetComponent<Image>().sprite = unlockedBackground;

            //set unlocked item color
            imgItemBackground.color = unlockedBackgroundColor;

            //set upgrade button info and color
            txtBtBuyUpgradeInfo.text = "Level Up";
            txtBtBuyUpgradeCost.text = invaderScrp.getUpgadeCost() + " Res.";

            if (StatsHandler.instance.Resources >= invaderScrp.getUpgadeCost())
                btBuyUpgrade.GetComponent<Image>().color = canBuyButtonColor;
            else
                btBuyUpgrade.GetComponent<Image>().color = cantBuyButtonColor;
        }
        else //when locked
        {
            //set locked item color
            imgItemBackground.color = lockedBackgroundColor;

            //set unlock button info and color
            txtBtBuyUpgradeInfo.text = "Unlock";
            txtBtBuyUpgradeCost.text = invaderScrp.unlockCost + " Res.";
            if (invaderScrp.getUnlockConditionsFullfilled() && StatsHandler.instance.Resources >= invaderScrp.unlockCost)
                btBuyUpgrade.GetComponent<Image>().color = canBuyButtonColor;
            else
                btBuyUpgrade.GetComponent<Image>().color = cantBuyButtonColor;
        }
    }

    private void onClickBtInfo()
    {
        PopupHandler.instance.showInvaderInfoPopup(invaderScrp, PopupHandler.PopupAlignment.Center);
    }

    private void addEventListeners()
    {
        if (gameObject.GetComponent<EventTrigger>() == null)
        {
            //Drag & drop
            EventTrigger trigger = imgInvader.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { OnDragInvader(invaderScrp); });
            trigger.triggers.Add(entry);

            //Pointer enter
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) =>
            {
                imgInvader.rectTransform.sizeDelta = hoverImgSize;
            });
            trigger.triggers.Add(entry);

            //Pointer exit
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((data) =>
            {
                imgInvader.rectTransform.sizeDelta = originImgSize;
            });
            trigger.triggers.Add(entry);
        }
    }

    public void refreshInfos()
    {
        setInfos();
        onBtBuyOrUpgrade();
    }


    private void OnDragInvader(Invader invaderScrp)
    {
        //check invader unlocked
        if (invaderScrp.unlocked)
        {
            //check free formation slots available
            if (FormationHandler.instance.freeFormationSlots >= invaderScrp.formationSlotSize.neededFormationSlots)
            {
                //check setted invaders limit not reached
                if (FormationHandler.instance.getSettedInvadersById(invaderScrp.id) < invaderScrp.maxInFormation)
                {
                    InvaderChooser.instance.createDragDropInvader(invaderScrp);
                }
                else //when setted invaders limit reached
                {
                    PopupHandler.instance.showPopupWithOKButton("Maximum number of " + invaderScrp.name + "s reached.");
                }
            }
            else //when no formation slots
            {
                PopupHandler.instance.showPopupWithOKButton("Not enough formation slots!");
                return;
            }
        }
        else //when invader lacked
        {
            PopupHandler.instance.showPopupWithOKButton("Invader locked!");
        }
    }

    private void onBtBuyOrUpgrade()
    {
        btBuyUpgrade.onClick.RemoveAllListeners();
        btBuyUpgrade.onClick.AddListener(() =>
        {
            if (invaderScrp.unlocked)
            {
                UpgradeInvader();
            }
            else if (!invaderScrp.unlocked)
            {
                buyInvader();
            }

        });
    }

    private void buyInvader()
    {
        uint curRes = StatsHandler.instance.Resources;
        uint buyCosts = invaderScrp.unlockCost;
        if (invaderScrp.unlockInvader())
        {
            StaticAudioHandler.playSound(SoundChooser.instance.buyInvader);

            InvaderChooser.instance.refreshList();
            FormationMenue.instance.refreshTxtResources(curRes, buyCosts);
        }
        else
        {
            StaticAudioHandler.playSound(SoundChooser.instance.cantUpgradeOrBuy);
        }
    }

    private void UpgradeInvader()
    {
        uint curRes = StatsHandler.instance.Resources;
        uint upgradeCosts = invaderScrp.getUpgadeCost();
        if (invaderScrp.upgrade())
        {
            StaticAudioHandler.playSound(SoundChooser.instance.upgradeInvader);

            InvaderChooser.instance.refreshList();
            FormationMenue.instance.refreshTxtResources(curRes, upgradeCosts);
        }
        else
        {
            StaticAudioHandler.playSound(SoundChooser.instance.cantUpgradeOrBuy);
        }
    }


    private string getWeaponInfosAsString(bool detailed, string frontString = null, string backString = null)
    {
        string weaponInfos = null;

        List<string> invaderWeaponInfos = new List<string>();
        if (invaderScrp.weapons != null)
        {
            string tmpInfo;
            tmpInfo = createWeaponInfo(invaderScrp.getWeaponsOfType(Weapon.WeaponType.Laser), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);

            tmpInfo = createWeaponInfo(invaderScrp.getWeaponsOfType(Weapon.WeaponType.Missle_launcher), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);

            tmpInfo = createWeaponInfo(invaderScrp.getWeaponsOfType(Weapon.WeaponType.Bomb), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);

            tmpInfo = createWeaponInfo(invaderScrp.getWeaponsOfType(Weapon.WeaponType.Scattering_laser), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);

            tmpInfo = createShieldInfo(invaderScrp.getShield(), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);

            tmpInfo = createSupportSystemInfo(invaderScrp.getSupportSystem(), detailed);
            if (tmpInfo != null)
                invaderWeaponInfos.Add(frontString + tmpInfo + backString);
        }
        if (invaderWeaponInfos.Count > 0)
        {
            weaponInfos = "";
            for (int i = 0; i < invaderWeaponInfos.Count; i++)
            {
                weaponInfos += invaderWeaponInfos[i];
                if (i + 1 < invaderWeaponInfos.Count && invaderWeaponInfos[i + 1] != null && invaderWeaponInfos[i + 1].Length > 0)//if next item has content
                {
                    weaponInfos += "\n";
                }
            }
        }

        return weaponInfos;
    }

    private string createWeaponInfo(ActiveWeapon[] activeWeapons, bool detailed)
    {
        if (activeWeapons == null || activeWeapons.Length == 0)
            return null;

        ActiveWeapon activeWeapon = activeWeapons[0];
        string weaponInfo = "";
        if (activeWeapons.Length == 1)
            weaponInfo += activeWeapon.weapon.GetComponent<Weapon>().weaponType.ToString().Replace("_", " ");
        else
            weaponInfo += activeWeapons.Length + "x " + activeWeapon.weapon.GetComponent<Weapon>().weaponType.ToString().Replace("_", " ");

        if (detailed)
        {
            weaponInfo += ":";
            weaponInfo += "\n\t\tCooldown: " + activeWeapon.cooldown + " s" + statsLevelFrontString + " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfo += "\n\t\tSpeed: " + activeWeapon.speed * 100 + " m/s" + statsLevelFrontString + " >> " + activeWeapon.getSpeedOfLevelN(invaderScrp.level + 1) * 100 + " m/s";
            weaponInfo += "\n\t\tDamage: " + activeWeapon.damage + statsLevelFrontString + " >> " + activeWeapon.getDamageOfLevelN(invaderScrp.level + 1);
        }

        if (weaponInfo.Length <= 0)
            return null;

        return weaponInfo;
    }

    private string createShieldInfo(ActiveWeapon activeWeapon, bool detailed)
    {
        if (activeWeapon == null)
            return null;

        string weaponInfo = "";

        weaponInfo += activeWeapon.weapon.GetComponent<Weapon>().weaponType.ToString().Replace("_", " ");

        if (detailed)
        {
            weaponInfo += ":";
            weaponInfo += "\n\t\tCooldown: " + activeWeapon.cooldown + " s" + statsLevelFrontString + " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfo += "\n\t\tHitpoints: " + activeWeapon.shieldHitpoints + statsLevelFrontString + " >> " + activeWeapon.getShieldHitpointsOfLevelN(invaderScrp.level + 1);
            weaponInfo += "\n\t\tLife time: " + activeWeapon.shieldTime + " s" + statsLevelFrontString + " >> " + activeWeapon.getShieldTimeOfLevelN(invaderScrp.level + 1) + " s";
        }

        if (weaponInfo.Length <= 0)
            return null;

        return weaponInfo;
    }

    private string createSupportSystemInfo(ActiveWeapon activeWeapon, bool detailed)
    {
        if (activeWeapon == null)
            return null;

        string weaponInfo = "";

        weaponInfo += activeWeapon.weapon.GetComponent<Weapon>().weaponType.ToString().Replace("_", " ");

        if (detailed)
        {
            weaponInfo += ":";
            weaponInfo += "\n\t\tCooldown: " + activeWeapon.cooldown + " s" + statsLevelFrontString + " >> " + activeWeapon.getCooldownOfLevelN(invaderScrp.level + 1) + " s";
            weaponInfo += "\n\t\tDuration: " + activeWeapon.effectDuration + " s" + statsLevelFrontString + " >> " + activeWeapon.getEffectDurationOfLevelN(invaderScrp.level + 1) + " s";

            if (activeWeapon.weaponType == Weapon.WeaponType.Repair_drone || activeWeapon.weaponType == Weapon.WeaponType.Collector_drone)
            {
                weaponInfo += "\n\t\tDrones: " + activeWeapon.dronesQuantity + statsLevelFrontString + " >> " + activeWeapon.getDronesQuantityOfLevelN(invaderScrp.level + 1);
                weaponInfo += "\n\t\tDrone speed: " + activeWeapon.speed * 100 + " m/s" + statsLevelFrontString + " >> " + activeWeapon.getSpeedOfLevelN(invaderScrp.level + 1) * 100 + " m/s";
            }
        }

        if (weaponInfo.Length <= 0)
            return null;

        return weaponInfo;
    }


}