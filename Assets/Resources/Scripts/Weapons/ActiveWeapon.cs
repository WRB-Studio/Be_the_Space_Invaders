using UnityEngine;

[System.Serializable]
public class ActiveWeapon
{


    [Header("Weapon settings")]
    public GameObject weapon;
    public Transform emitterTransform;
    public Weapon.WeaponType weaponType { get; private set; }
    public Invader invaderScrp { get; private set; }
    public bool isInvaderWeapon { get; private set; }


    [Header("Weapon stats")]
    public float cooldownMin;
    public float cooldownDefaultLVL1;
    public float cooldown { get; private set; }
    public float cooldownCounter { get; private set; }

    public float speedMax;
    public float speedDefaultLVL1;
    public float speed { get; private set; }

    public int damageMultipliesOf = 1;
    public int damageDefaultLVL1;
    public int damage { get; private set; }


    [Header("Shield stats")]
    public float shieldHitpointsMultiplier;
    public int shieldHitpoints { get; private set; }
    public float shieldTimeMax;
    public float shieldTimeDefaultLVL1;
    public float shieldTime { get; private set; }

    public GameObject instantiatedShield { get; private set; }


    [Header("Supporter stats")]
    public float effectDurationMax;
    public float effectDurationDefaultLVL1;
    public float effectDuration { get; private set; }

    public int hitpointsPerSecMultipliesOf;
    public int hitpointsPerSecDefaultLVL1;
    public int hitpointsPerSec { get; private set; }

    public int dronesQuantityMax;
    public int dronesQuantityMultipliesOf;
    public int dronesQuantityDefaultLVL1;
    public int dronesQuantity { get; private set; }

    public int resourceExtractionMultipliesOf;
    public int resourceExtractionDefaultLVL1;
    public uint resourceExtraction { get; private set; }

    public GameObject instantiatedSupportSystem { get; private set; }



    public void init(Invader invaderScrpVal = null)
    {
        invaderScrp = invaderScrpVal;
        if (invaderScrp != null)
            isInvaderWeapon = true;

        weaponType = weapon.GetComponent<Weapon>().weaponType;

        if (invaderScrp == null)
            setDefaultValues();

        cooldownCounter = cooldown / 2;
    }


    public void updateCall()
    {
        if (weaponType == Weapon.WeaponType.Repair_drone || weaponType == Weapon.WeaponType.Collector_drone || weaponType == Weapon.WeaponType.Targeting_system) //when support system
        {
            if (instantiatedSupportSystem != null)
            {
                instantiatedSupportSystem.GetComponent<SupportSystem>().updateCall();
            }
            else
            {
                if (cooldownCounter > 0)
                    cooldownCounter -= Time.fixedDeltaTime;
            }
        }
        else if (weaponType == Weapon.WeaponType.Absorbing_shield || weaponType == Weapon.WeaponType.Deflector_shield) //when shield
        {
            if (instantiatedShield != null)
            {
                instantiatedShield.GetComponent<Shield>().updateCall();
            }
            else
            {
                if (cooldownCounter > 0)
                    cooldownCounter -= Time.fixedDeltaTime;
            }
        }
        else //when shoting weapon
        {
            if (cooldownCounter > 0)
                cooldownCounter -= Time.fixedDeltaTime;
        }
    }

    public bool cooldownFinished()
    {
        if (cooldownCounter <= 0)
            return true;

        return false;
    }

    public void shoot()
    {
        if (cooldownCounter <= 0)
        {
            ShootHandler.instance.instantiateShoot(this, emitterTransform.position);
            cooldownCounter = cooldown;
        }
    }

    public GameObject missleShoot(GameObject targetObject)
    {
        if (cooldownCounter <= 0)
        {
            GameObject missleShoot = ShootHandler.instance.instantiateMissleShoot(this, emitterTransform.position, targetObject);
            cooldownCounter = cooldown;

            return missleShoot;
        }

        return null;
    }


    public void startShield()
    {
        if (instantiatedShield != null)
            return;

        instantiatedShield = Shield.instantiateShield(this);
        instantiatedShield.transform.parent = emitterTransform;
        instantiatedShield.transform.position = emitterTransform.position;

        cooldownCounter = cooldown;
    }

    public bool startSupportSystem(Invader targetInvaderScrp = null, GameObject crosshair = null)
    {
        if (instantiatedSupportSystem != null || !cooldownFinished())
            return false;

        instantiatedSupportSystem = SupportSystem.instantiateSupportSystem(this, targetInvaderScrp);
        if (instantiatedSupportSystem != null)
        {
            if (crosshair != null)
                crosshair.transform.parent = instantiatedSupportSystem.transform;

            cooldownCounter = cooldown;

            return true;
        }
        else
        {
            return false;
        }
    }


    #region upgrade methods

    public void upgrade(int upgradeLevel)
    {
        if (isInvaderWeapon)
        {
            cooldown = getCooldownOfLevelN(upgradeLevel);

            if (invaderScrp.invaderClass == Invader.Invaderclass.Attacker || invaderScrp.invaderClass == Invader.Invaderclass.Boss)
            {
                speed = getSpeedOfLevelN(upgradeLevel);
                damage = getDamageOfLevelN(upgradeLevel);
            }
            else if (invaderScrp.invaderClass == Invader.Invaderclass.Defender)
            {
                shieldHitpoints = getShieldHitpointsOfLevelN(upgradeLevel);
                shieldTime = getShieldTimeOfLevelN(upgradeLevel);
            }
            else if (invaderScrp.invaderClass == Invader.Invaderclass.Supporter)
            {
                effectDuration = getEffectDurationOfLevelN(upgradeLevel);
                dronesQuantity = getDronesQuantityOfLevelN(upgradeLevel);

                if (invaderScrp.getSupportSystem().weaponType != Weapon.WeaponType.Targeting_system)
                    speed = getSpeedOfLevelN(upgradeLevel);

                if (invaderScrp.getSupportSystem().weaponType == Weapon.WeaponType.Repair_drone)
                    hitpointsPerSec = getHitpointsPerSecOfLevelN(upgradeLevel);

                if (invaderScrp.getSupportSystem().weaponType == Weapon.WeaponType.Collector_drone)
                    resourceExtraction = getResourceCollectingExtractionOfLevelN(upgradeLevel);
            }
        }
        else
        {
            cooldown = getCooldownOfLevelN(upgradeLevel);
            speed = getSpeedOfLevelN(upgradeLevel);
            damage = getDamageOfLevelN(upgradeLevel);
        }
    }

    private void setDefaultValues()
    {
        speed = speedDefaultLVL1;
        cooldown = cooldownDefaultLVL1;
        damage = damageDefaultLVL1;
        if (invaderScrp != null)
            shieldHitpoints = getShieldHitpointsOfLevelN(0);
        shieldTime = shieldTimeDefaultLVL1;
        effectDuration = effectDurationDefaultLVL1;
        hitpointsPerSec = hitpointsPerSecDefaultLVL1;
        dronesQuantity = dronesQuantityDefaultLVL1;
        resourceExtraction = (uint)resourceExtractionDefaultLVL1;
    }


    public float getCooldownOfLevelN(int level)
    {
        float defaultLVL1Val = cooldownDefaultLVL1;
        float calculatedVal = cooldownDefaultLVL1;
        float minOrMaxValue = cooldownMin;

        for (int currentLevelIndex = 1; currentLevelIndex < level; currentLevelIndex++)
        {
            calculatedVal = calculatedVal - ((defaultLVL1Val - minOrMaxValue) / 100);

            if (calculatedVal < minOrMaxValue)
            {
                calculatedVal = minOrMaxValue;
                break;
            }

        }
        //Debug.Log(level + " | " + minOrMaxValue + " | " + calculatedVal);

        return (float)System.Math.Round(calculatedVal, 2);
    }

    public float getSpeedOfLevelN(int level)
    {
        float defaultLVL1Val = speedDefaultLVL1;
        float calculatedVal = speedDefaultLVL1;
        float minOrMaxValue = speedMax;

        for (int currentLevelIndex = 1; currentLevelIndex < level; currentLevelIndex++)
        {
            calculatedVal = calculatedVal + ((minOrMaxValue - defaultLVL1Val) / 100);

            if (calculatedVal > minOrMaxValue)
            {
                calculatedVal = minOrMaxValue;
                break;
            }
        }
        //Debug.Log(level + " | " + minOrMaxValue + " | " + calculatedVal);

        return (float)System.Math.Round(calculatedVal, 2);
    }

    public int getDamageOfLevelN(int level)
    {
        float defaultLVL1Val = damageDefaultLVL1;
        float calculatedVal = damageDefaultLVL1;
        int multipliesOf = damageMultipliesOf;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, multipliesOf))
            {
                calculatedVal = calculatedVal + defaultLVL1Val;
            }
        }
        //Debug.Log(level + " | " + multipliesOf + " | " + calculatedVal);

        return (int)System.Math.Round(calculatedVal, 0);
    }


    public int getShieldHitpointsOfLevelN(int level)
    {
        float calculatedVal = invaderScrp.hitpointsDefaultLVL1;
        float multiplier = shieldHitpointsMultiplier;

        calculatedVal = invaderScrp.getHitpointsOfLevelN(level) * multiplier;

        return (int)System.Math.Round(calculatedVal, 0);
    }

    public float getShieldTimeOfLevelN(int level)
    {
        float defaultLVL1Val = shieldTimeDefaultLVL1;
        float calculatedVal = shieldTimeDefaultLVL1;
        float minOrMaxValue = shieldTimeMax;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            calculatedVal = calculatedVal + ((minOrMaxValue - defaultLVL1Val) / 100);

            if (calculatedVal > minOrMaxValue)
            {
                calculatedVal = minOrMaxValue;
                break;
            }
        }
        //Debug.Log(level + " | " + minOrMaxValue + " | " + calculatedVal);

        return (float)System.Math.Round(calculatedVal, 2);
    }


    public float getEffectDurationOfLevelN(int level)
    {
        float defaultLVL1Val = effectDurationDefaultLVL1;
        float calculatedVal = effectDurationDefaultLVL1;
        float minOrMaxValue = effectDurationMax;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            calculatedVal = calculatedVal + ((minOrMaxValue - defaultLVL1Val) / 100);

            if (calculatedVal > minOrMaxValue)
            {
                calculatedVal = minOrMaxValue;
                break;
            }
        }
        //Debug.Log(level + " | " + minOrMaxValue + " | " + calculatedVal);

        return (float)System.Math.Round(calculatedVal, 2);
    }

    public int getHitpointsPerSecOfLevelN(int level)
    {
        float defaultLVL1Val = hitpointsPerSecDefaultLVL1;
        float calculatedVal = hitpointsPerSecDefaultLVL1;
        int multipliesOf = hitpointsPerSecMultipliesOf;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, multipliesOf))
            {
                calculatedVal = calculatedVal + 1;
            }
        }
        //Debug.Log(level + " | " + multiplier + " | " + calculatedVal);

        return (int)System.Math.Round(calculatedVal, 0);
    }

    public int getDronesQuantityOfLevelN(int level)
    {
        float calculatedVal = dronesQuantityDefaultLVL1;
        int multipliesOf = dronesQuantityMultipliesOf;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, multipliesOf))
            {
                calculatedVal = calculatedVal + 1;
            }
            //Debug.Log("Drones quantity: " + currentLevelIndex + " | " + calculatedVal);
        }

        return (int)System.Math.Round(calculatedVal, 0);
    }


    public uint getResourceCollectingExtractionOfLevelN(int level)
    {
        float calculatedVal = resourceExtractionDefaultLVL1;
        int updateMultipliesOf = resourceExtractionMultipliesOf;
        int addCalcMultipliesOf = 5;
        float resAddCalc = 1.5f;

        for (int currentLevelIndex = 1; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, 10))
            {
                calculatedVal += currentLevelIndex / 2.5f;
            }

            if (MyUtilities.isMultipleOf(currentLevelIndex, addCalcMultipliesOf))
            {
                resAddCalc += 2;
            }

            if (MyUtilities.isMultipleOf(currentLevelIndex, updateMultipliesOf))
            {
                calculatedVal = Mathf.RoundToInt(calculatedVal + 1.5f);
            }

            //Debug.Log("Res. extract: " + currentLevelIndex + " | " + calculatedVal);
        }

        return (uint)System.Math.Round(calculatedVal, 0);
    }

    #endregion


    #region Save and Load methods

    public WeaponData save()
    {
        WeaponData weaponData = new WeaponData();

        weaponData.invaderID = invaderScrp.id;
        weaponData.weaponType = (int)weapon.GetComponent<Weapon>().weaponType;

        /*weaponData.speed = speed;
        weaponData.cooldown = cooldown;
        weaponData.damage = damage;

        weaponData.shieldHitpoints = shieldHitpoints;
        weaponData.shieldTime = shieldTime;

        weaponData.effectDuration = effectDuration;
        weaponData.hitpointsPerSec = hitpointsPerSec;
        weaponData.dronesQuantity = dronesQuantity;

        weaponData.resourceExtraction = resourceExtraction;*/

        return weaponData;
    }

    public void load(WeaponData data, Invader invaderScrpVal)
    {
        invaderScrp = invaderScrpVal;

        isInvaderWeapon = true;

        weaponType = weapon.GetComponent<Weapon>().weaponType;

        upgrade(invaderScrp.level);

        /*speed = data.speed;
        cooldown = data.cooldown;
        damage = data.damage;

        shieldHitpoints = data.shieldHitpoints;
        shieldTime = data.shieldTime;

        effectDuration = data.effectDuration;
        hitpointsPerSec = data.hitpointsPerSec;
        dronesQuantity = data.dronesQuantity;

        resourceExtraction = data.resourceExtraction;*/
    }

    #endregion
}