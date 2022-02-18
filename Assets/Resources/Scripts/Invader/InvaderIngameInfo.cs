using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvaderIngameInfo : MonoBehaviour
{
    public Color cooldownColor;
    public Color durationColor;

    public Slider sliderHitpoints;

    public Image imgLaser;
    public Image imgMissle;
    public Image imgBomb;
    public Image imgShield;
    public Image imgSupportsystem;

    private Image laserProgressCircle;
    private Image missleProgressCircle;
    private Image bombProgressCircle;
    private Image shieldProgressCircle;
    private Image supportSystemProgressCircle;

    private ActiveWeapon[] laserWeapons;
    private ActiveWeapon[] missleWeapons;
    private ActiveWeapon[] bombWeapons;
    private ActiveWeapon shield;
    private ActiveWeapon supportSystem;


    private Invader invaderScrp;



    public void init(Invader invaderScrpVal)
    {
        invaderScrp = invaderScrpVal;

        sliderHitpoints.maxValue = invaderScrp.hitpointsCurrentLevelDefault;

        laserProgressCircle = imgLaser.transform.GetChild(0).GetComponent<Image>();
        missleProgressCircle = imgMissle.transform.GetChild(0).GetComponent<Image>();
        bombProgressCircle = imgBomb.transform.GetChild(0).GetComponent<Image>();
        shieldProgressCircle = imgShield.transform.GetChild(0).GetComponent<Image>();
        supportSystemProgressCircle = imgSupportsystem.transform.GetChild(0).GetComponent<Image>();

        laserProgressCircle.color = cooldownColor;
        missleProgressCircle.color = cooldownColor;
        bombProgressCircle.color = cooldownColor;
        shieldProgressCircle.color = cooldownColor;
        supportSystemProgressCircle.color = cooldownColor;

        laserWeapons = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Laser);
        missleWeapons = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Missle_launcher);
        bombWeapons = invaderScrp.getWeaponsOfType(Weapon.WeaponType.Bomb);
        shield = invaderScrp.getShield();
        supportSystem = invaderScrp.getSupportSystem();

        if (laserWeapons == null)
        {
            laserWeapons = null;
            imgLaser.gameObject.SetActive(false);
        }
        if (missleWeapons == null)
        {
            missleWeapons = null;
            imgMissle.gameObject.SetActive(false);
        }
        if (bombWeapons == null)
        {
            bombWeapons = null;
            imgBomb.gameObject.SetActive(false);
        }
        if (shield == null)
        {
            shield = null;
            imgShield.gameObject.SetActive(false);
        }
        if (supportSystem == null)
        {
            supportSystem = null;
            imgSupportsystem.gameObject.SetActive(false);
        }
    }

    public void updateCall()
    {
        sliderHitpoints.value = invaderScrp.currentHitpoints;

        setCooldownInfosForWeapons(laserWeapons, laserProgressCircle);
        setCooldownInfosForWeapons(missleWeapons, missleProgressCircle);
        setCooldownInfosForWeapons(bombWeapons, bombProgressCircle);
        setCooldownInfoForShield(shield, shieldProgressCircle);
        setCooldownInfosForSupportSystem(supportSystem, supportSystemProgressCircle);
    }

    private void setCooldownInfosForWeapons(ActiveWeapon[] weapons, Image progressCircleImage)
    {
        if (weapons == null)
            return;

        ActiveWeapon weaponWithShortestCooldown = getWeaponWithShortestColldown(weapons);
        float cooldown = weaponWithShortestCooldown.cooldown;
        float currentCooldown = weaponWithShortestCooldown.cooldownCounter;
        float cooldownInPercent = currentCooldown / (cooldown / 100);

        progressCircleImage.fillAmount = cooldownInPercent / 100;
    }

    private void setCooldownInfoForShield(ActiveWeapon weapon, Image progressCircleImage)
    {
        if (weapon == null)
            return;

        if (weapon.instantiatedShield == null)
        {
            progressCircleImage.color = cooldownColor;
            progressCircleImage.fillClockwise = true;

            float cooldown = weapon.cooldown;
            float currentCooldown = weapon.cooldownCounter;
            float cooldownInPercent = currentCooldown / (cooldown / 100);

            progressCircleImage.fillAmount = cooldownInPercent / 100;
        }
        else
        {
            progressCircleImage.color = durationColor;
            progressCircleImage.fillClockwise = false;

            Shield shieldinstance = weapon.instantiatedShield.GetComponent<Shield>();
            float shieldTime = shieldinstance.shieldTime;
            float currentShieldTime = shieldinstance.currentShieldTime;
            float shieldTimeInPercent = currentShieldTime / (shieldTime / 100);

            progressCircleImage.fillAmount = shieldTimeInPercent / 100;
        }
    }

    private void setCooldownInfosForSupportSystem(ActiveWeapon weapon, Image progressCircleImage)
    {
        if (weapon == null)
            return;

        if (weapon.instantiatedSupportSystem == null)
        {
            progressCircleImage.color = cooldownColor;
            progressCircleImage.fillClockwise = true;

            float cooldown = weapon.cooldown;
            float currentCooldown = weapon.cooldownCounter;
            float cooldownInPercent = currentCooldown / (cooldown / 100);

            progressCircleImage.fillAmount = cooldownInPercent / 100;
        }
        else
        {
            progressCircleImage.color = durationColor;
            progressCircleImage.fillClockwise = false;

            SupportSystem supportSysteminstance = weapon.instantiatedSupportSystem.GetComponent<SupportSystem>();
            float effectDuration = supportSysteminstance.effectDuration;
            float currentEffectDuration = supportSysteminstance.effectDurationCounter;
            float effectDurationInPercent = currentEffectDuration / (effectDuration / 100);

            progressCircleImage.fillAmount = effectDurationInPercent / 100;
        }
    }



    private ActiveWeapon getWeaponWithShortestColldown(ActiveWeapon[] weapons)
    {
        float shortestCooldown = 100;
        ActiveWeapon weaponWithShortestCooldown = null;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].cooldownCounter < shortestCooldown)
            {
                shortestCooldown = weapons[i].cooldownCounter;
                weaponWithShortestCooldown = weapons[i];
            }
        }

        return weaponWithShortestCooldown;
    }

}
