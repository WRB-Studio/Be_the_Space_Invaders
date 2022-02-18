using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Invader : MonoBehaviour
{
    public enum Invaderclass
    {
        None, Attacker, Defender, Boss, Supporter
    }

    [TextArea(3, 5)]
    public string invaderInfo;

    public bool isInfoinstance = true;

    [Header("Invader infos")]
    public int id;
    public Invaderclass invaderClass = Invaderclass.None;
    public int level;
    public int maxInFormation;
    public InvaderFormationSlotSize formationSlotSize;

    [Header("Hitpoint settings")]
    public int hitpointsMultipliesOf = 1;
    public int hitpointsDefaultLVL1;
    public int hitpointsCurrentLevelDefault { get; private set; }
    public int currentHitpoints { get; private set; }

    [Header("Unlock & Upgrade settings")]

    public bool unlocked;
    private bool unlockConditionsFullfilled = false;
    public uint unlockCost = 0;
    public float upgradeCostDefaultMultiplier = 1;
    public int upgradeCostMultipliesOf = 1;
    public int upgradeCostLVL1 = 1;

    [Header("Animation settings")]
    public GameObject explosionEffectPrefab;

    [Header("Weapon settings")]
    public ActiveWeapon[] weapons;
    private InvaderIngameInfo invaderIngameInfoScrp;

    [Header("Aiming drag settings")]
    public float aimingDetectionRadius = 1;
    private GameObject crossHairPrefab;
    private GameObject instantiatedCrossHair;
    private bool aimingDragCoroutineProcess = false;
    private bool invaderSpriteEnter = false;

    public SpriteRenderer invaderTexture { get { return transform.GetChild(0).GetComponent<SpriteRenderer>(); } }



    #region init methods

    public void init(bool isInfoinstanceVal)
    {
        crossHairPrefab = Resources.Load<GameObject>("Prefabs/Weapons/CrossHair");
        isInfoinstance = isInfoinstanceVal;

        currentHitpoints = hitpointsDefaultLVL1;

        load();

        hitpointsCurrentLevelDefault = currentHitpoints;

        if (!isInfoinstance)
        {
            transform.GetChild(0).gameObject.AddComponent<InvaderColliderHandler>().init(this);
            repositioningAndScalingInvaderSprite(transform.GetChild(0), formationSlotSize.formationSlotSize);
            createEventHandler();
            initInvaderInfoUI(transform.GetChild(0));
        }


        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].init(this);
        }
    }

    public void updateCall()
    {
        if (weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].updateCall();

        invaderIngameInfoScrp.updateCall();
    }

    private void repositioningAndScalingInvaderSprite(Transform spriteTransform, int[] invaderFormationSize)
    {
        spriteTransform.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        Sprite invaderTexture = spriteTransform.GetComponent<SpriteRenderer>().sprite;
        //spriteTransform.GetComponent<SpriteRenderer>().color = textureColor;
        float sizeX = invaderTexture.bounds.size.x;
        float sizeY = invaderTexture.bounds.size.y;

        float scaleFactor = 1.6f;
        spriteTransform.transform.localScale = new Vector3(spriteTransform.transform.localScale.x / scaleFactor, spriteTransform.transform.localScale.y / scaleFactor, spriteTransform.transform.localScale.z / scaleFactor);

        int invaderRowSlots = formationSlotSize.formationSlotSize[0];
        int invaderColumnSlots = formationSlotSize.formationSlotSize[1];
        spriteTransform.localPosition = new Vector2(spriteTransform.localPosition.x + (invaderColumnSlots - 1) * (sizeX / scaleFactor / invaderColumnSlots / 2),
                                                    spriteTransform.localPosition.y - (invaderRowSlots - 1) * (sizeY / scaleFactor / invaderRowSlots / 2));

        spriteTransform.gameObject.AddComponent<PolygonCollider2D>();
        spriteTransform.tag = "Invader";
    }

    private void createEventHandler()
    {
        GameObject eventHandlerGO = new GameObject("EventHandlerGO");
        eventHandlerGO.transform.parent = transform;

        InvaderEventHandler invaderEventHandler = eventHandlerGO.AddComponent<InvaderEventHandler>();
        invaderEventHandler.init(this);
    }

    private void initInvaderInfoUI(Transform spriteTransform)
    {
        GameObject canvasInvaderInfo = Resources.Load<GameObject>("Prefabs/UI/CanvasInvaderInfo");
        canvasInvaderInfo = Instantiate(canvasInvaderInfo, spriteTransform);
        Vector3 position = transform.position;
        position.x -= 0.25f;
        //position.y += 0.25f;
        canvasInvaderInfo.transform.position = position;

        canvasInvaderInfo.GetComponent<InvaderIngameInfo>().init(this);

        invaderIngameInfoScrp = canvasInvaderInfo.GetComponent<InvaderIngameInfo>();
    }

    #endregion


    #region Weapon methods  

    public ActiveWeapon[] getWeaponsOfType(Weapon.WeaponType weapontype)
    {
        List<ActiveWeapon> weaponsOfType = new List<ActiveWeapon>();
        for (int weaponIndex = 0; weaponIndex < weapons.Length; weaponIndex++)
        {
            if (weapons[weaponIndex].weapon.GetComponent<Weapon>().weaponType == weapontype)
                weaponsOfType.Add(weapons[weaponIndex]);
        }

        if (weaponsOfType.Count > 0)
            return weaponsOfType.ToArray();

        return null;
    }

    public ActiveWeapon[] getWeapons(int primarySecondaryTertiary)
    {
        if (weapons == null)
            return null;

        ActiveWeapon searchedWeapon = weapons[0];
        int countWeaponTypes = 1;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weaponType != searchedWeapon.weaponType)
            {
                countWeaponTypes++;
            }

            if (countWeaponTypes == primarySecondaryTertiary)
            {
                searchedWeapon = weapons[i];
                break;
            }
        }

        return getWeaponsOfType(searchedWeapon.weaponType);
    }

    public ActiveWeapon getShield()
    {
        if (invaderClass == Invaderclass.Defender && weapons != null && weapons.Length > 0)
            return weapons[0];

        return null;
    }

    public ActiveWeapon getSupportSystem()
    {
        if (invaderClass == Invaderclass.Supporter && weapons != null && weapons.Length > 0)
            return weapons[0];

        return null;
    }

    public int numberOfWeaponTypes()
    {
        if (weapons == null || weapons.Length < 1)
            return 0;

        List<ActiveWeapon> weaponTypes = new List<ActiveWeapon>();
        bool weaponTypeExistInList = false;
        for (int weaponsIndex = 0; weaponsIndex < weapons.Length; weaponsIndex++)
        {
            weaponTypeExistInList = false;
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                if (weapons[weaponsIndex].weapon.GetComponent<Weapon>().weaponType == weaponTypes[i].weapon.GetComponent<Weapon>().weaponType)
                {
                    weaponTypeExistInList = true;
                    break;
                }
            }
            if (!weaponTypeExistInList)
                weaponTypes.Add(weapons[weaponsIndex]);
        }

        return weaponTypes.Count;
    }


    public virtual void onSingleClick()
    {
        if (isInfoinstance || weapons == null || numberOfWeaponTypes() <= 0 || IngameHandler.instance.isPauseOrGameOver())
            return;

        ActiveWeapon[] currentWeapons = getWeapons(1);

        switch (currentWeapons[0].weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Bomb:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Missle_launcher:
                shootAutomaticMissile(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Absorbing_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Deflector_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Collector_drone:
                startSupportSystem(currentWeapons[0]);
                break;
            default:
                break;
        }
    }

    public virtual void onDoubleClick()
    {
        if (isInfoinstance || weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        if (numberOfWeaponTypes() == 1)
        {
            onSingleClick();
            return;
        }

        ActiveWeapon[] currentWeapons = getWeapons(2);

        switch (currentWeapons[0].weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Bomb:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Missle_launcher:
                shootAutomaticMissile(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Absorbing_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Deflector_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Collector_drone:
                startSupportSystem(currentWeapons[0]);
                break;
            default:
                break;
        }
    }

    public virtual void onTrippleClick()
    {
        if (isInfoinstance || weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        if (numberOfWeaponTypes() == 1)
        {
            onSingleClick();
            return;
        }
        else if (numberOfWeaponTypes() == 2)
        {
            onDoubleClick();
            return;
        }

        ActiveWeapon[] currentWeapons = getWeapons(3);

        switch (currentWeapons[0].weaponType)
        {
            case Weapon.WeaponType.None:
                break;
            case Weapon.WeaponType.Laser:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Bomb:
                shootWeapons(currentWeapons);
                break;
            case Weapon.WeaponType.Missle_launcher:
                shootAutomaticMissile(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Absorbing_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Deflector_shield:
                startShield(currentWeapons[0]);
                break;
            case Weapon.WeaponType.Collector_drone:
                startSupportSystem(currentWeapons[0]);
                break;
            default:
                break;
        }
    }

    public virtual void multipleClick()
    {
        if (isInfoinstance || weapons == null || IngameHandler.instance.isPauseOrGameOver())
            return;

        switch (numberOfWeaponTypes())
        {
            case 0:

                break;
            case 1:
                onSingleClick();
                break;
            case 2:
                onDoubleClick();
                break;
            case 3:
                onTrippleClick();
                break;
            default:
                break;
        }
    }


    private bool shootWeapons(ActiveWeapon[] weapon)
    {
        if (weapon == null)
            return false;

        bool shooted = false;

        for (int i = 0; i < weapon.Length; i++)
        {
            if (weapon[i].cooldownFinished())
            {
                weapon[i].shoot();
                StatsHandler.instance.addShotsFired(1, name);
                shooted = true;
            }
        }

        return shooted;
    }

    private bool shootAutomaticMissile(ActiveWeapon missile)
    {
        if (missile == null)
            return false;

        if (missile.cooldownFinished())
        {
            //shoot missile           
            missile.missleShoot(null);

            StatsHandler.instance.addShotsFired(1, name);

            return true;
        }

        return false;
    }

    private bool startShield(ActiveWeapon shield)
    {
        if (shield == null)
            return false;

        if (shield.cooldownFinished())
        {
            shield.startShield();
            StatsHandler.instance.addUsesByInvaderType(name);
            return true;
        }

        return false;
    }

    private bool startSupportSystem(ActiveWeapon supportSystem)
    {
        if (supportSystem == null)
            return false;

        if (supportSystem.weaponType == Weapon.WeaponType.Collector_drone && supportSystem.cooldownFinished())
        {
            supportSystem.startSupportSystem();
            StatsHandler.instance.addUsesByInvaderType(name);
            return true;
        }

        return false;
    }


    /*--Aiming methods--*/

    public virtual void startAimingDrag()
    {
        if (aimingDragCoroutineProcess)
            return;

        ActiveWeapon[] missleLauncher = getWeaponsOfType(Weapon.WeaponType.Missle_launcher);
        ActiveWeapon supportSystem = getSupportSystem();

        if (missleLauncher != null)
        {
            for (int i = 0; i < missleLauncher.Length; i++)
            {
                if (missleLauncher[i].cooldownFinished())
                {
                    StartCoroutine(missileLauncherDragCoroutine(missleLauncher[i]));
                    break;
                }
            }
        }
        else if (supportSystem != null && supportSystem.weaponType != Weapon.WeaponType.Collector_drone)
        {
            if (supportSystem.cooldownFinished())
            {
                StartCoroutine(supporterAimingDragCoroutine(supportSystem));
            }
        }
    }

    public void stopAimingDrag()
    {
        //breake missle aiming drag coroutine
        if (aimingDragCoroutineProcess)
            aimingDragCoroutineProcess = false;
    }

    public void onEnterInvaderSprite()
    {
        invaderSpriteEnter = true;
    }

    public void onExitInvaderSprite()
    {
        invaderSpriteEnter = false;
    }

    protected virtual IEnumerator missileLauncherDragCoroutine(ActiveWeapon activeWeapon)
    {
        aimingDragCoroutineProcess = true;

        Vector3 pointerWorldPositon = MyUtilities.getMouseWorldPosition();

        GameObject targetObject = null;
        GameObject emptyTargetWaypoint = new GameObject("tmp_missileTarget_waypoint");

        instantiatedCrossHair = Instantiate(crossHairPrefab, pointerWorldPositon, crossHairPrefab.transform.rotation);
        instantiatedCrossHair.SetActive(false);
        instantiatedCrossHair.name = "tmp_Crosshair";
        Crosshair crossHairScrp = instantiatedCrossHair.GetComponent<Crosshair>();
        crossHairScrp.followPositionUpdate(pointerWorldPositon);

        /*
         * Crosshair follow mouse position when no target.
         * If pointer is near terran, asteroids or terran shots, the crosshair is attached to it.
         * If the pointer is released near the invader, the aiming process will stop without a shot.
         */
        while (aimingDragCoroutineProcess)
        {
            yield return null;

            pointerWorldPositon = MyUtilities.getMouseWorldPosition();

            //get target near pointer
            targetObject = getMissileLauncherAimingTargetByPointer(pointerWorldPositon);

            //if no target near pointer -> set empty target waypoint
            if (targetObject == null)
                targetObject = emptyTargetWaypoint;

            //if pointer enter on this invader -> hide crosshair
            if (invaderSpriteEnter)
            {
                targetObject = null;
                crossHairScrp.gameObject.SetActive(false);
            }
            else if (!invaderSpriteEnter) //if pointer is not over this invader -> show crosshair
            {
                crossHairScrp.gameObject.SetActive(true);

                //if empty target -> follow pointer position
                if (targetObject.name == "tmp_missileTarget_waypoint")
                    targetObject.transform.position = pointerWorldPositon;

                crossHairScrp.followPositionUpdate(targetObject.transform.position);
            }
        }

        //when target exist
        if (targetObject != null)
        {
            if (targetObject == emptyTargetWaypoint) //empty target waypoint
            {
                instantiatedCrossHair.transform.parent = targetObject.transform;
                activeWeapon.missleShoot(targetObject);
            }
            else //Concrete target
            {
                instantiatedCrossHair.transform.parent = targetObject.transform;
                crossHairScrp.initFollowTargetByMissle(targetObject, activeWeapon.missleShoot(targetObject));
            }

            if (instantiatedCrossHair != null)
                Destroy(instantiatedCrossHair);

            StatsHandler.instance.addShotsFired(1, name);
        }
        else //destroy when no target
        {
            if (emptyTargetWaypoint != null)
                Destroy(emptyTargetWaypoint);

            if (instantiatedCrossHair != null)
                Destroy(instantiatedCrossHair);
        }

        instantiatedCrossHair = null;
    }

    protected virtual GameObject getMissileLauncherAimingTargetByPointer(Vector3 mousePosition)
    {
        //Search target near the pointer position

        List<GameObject> instantiatedEnemys = new List<GameObject>();
        instantiatedEnemys = MyUtilities.addListToList(instantiatedEnemys, TerranShipHandler.instance.getInstantiatedTerranShips());
        //instantiatedEnemys.Add(TerranStationHandler.instance.getInstantiatedTerranStation());
        instantiatedEnemys = MyUtilities.addListToList(instantiatedEnemys, AsteroidHandler.instance.getInstantiatedAsteroids());
        instantiatedEnemys = MyUtilities.addListToList(instantiatedEnemys, ShootHandler.instance.getInstantiatedTerranShoots());

        float nearestObjectDistance = 0.5f;
        GameObject nearestObject = null;
        for (int i = 0; i < instantiatedEnemys.Count; i++)
        {
            if (instantiatedEnemys[i] == null)
                continue;

            float currentMouseToObjectDistance = Vector3.Distance(mousePosition, instantiatedEnemys[i].transform.position);
            if (currentMouseToObjectDistance <= aimingDetectionRadius && currentMouseToObjectDistance < nearestObjectDistance)
            {
                nearestObjectDistance = currentMouseToObjectDistance;
                nearestObject = instantiatedEnemys[i];
            }
        }

        return nearestObject;
    }

    protected virtual IEnumerator supporterAimingDragCoroutine(ActiveWeapon activeWeapon)
    {
        aimingDragCoroutineProcess = true;

        Vector3 mouseWorldPositon = MyUtilities.getMouseWorldPosition();

        GameObject targetObject = null;

        instantiatedCrossHair = Instantiate(crossHairPrefab, mouseWorldPositon, crossHairPrefab.transform.rotation);
        instantiatedCrossHair.SetActive(false);
        instantiatedCrossHair.name = "tmp_SupportCrosshair";
        Crosshair crossHairScrp = instantiatedCrossHair.GetComponent<Crosshair>();
        crossHairScrp.GetComponent<SpriteRenderer>().color = Color.green;
        crossHairScrp.followPositionUpdate(mouseWorldPositon);
        bool showCrossair = false;

        /*
         * Crosshair follow mouse position when no target.
         * If pointer is near invader ship from class attacker or Boss, the crosshair is attached to it.
         * If the pointer is released near the invader or empty place, the aiming process will stop without a shot.
         */
        while (aimingDragCoroutineProcess)
        {
            yield return null;

            mouseWorldPositon = MyUtilities.getMouseWorldPosition();

            targetObject = getSupporterAimingDragTargetByPointerPosition(mouseWorldPositon);

            if (targetObject == null) //if no target found => crosshair is green and follow pointer
            {
                crossHairScrp.followPositionUpdate(mouseWorldPositon);
                showCrossair = true;
                crossHairScrp.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if (targetObject != null) //target found
            {
                if (targetObject.tag == "Invader")//if target object is invader
                {
                    Invader targetInvaderScrp = targetObject.GetComponent<Invader>();


                    if (targetInvaderScrp == this) //check target invader is this invader
                    {
                        showCrossair = false;
                        targetObject = null;
                    }
                    else
                    {
                        showCrossair = true;

                        if (SupportSystem.canSupport(targetInvaderScrp, activeWeapon))
                        {
                            crossHairScrp.GetComponent<SpriteRenderer>().color = Color.green;
                            crossHairScrp.followPositionUpdate(targetObject.transform.GetChild(0).position);
                        }
                        else
                        {
                            crossHairScrp.GetComponent<SpriteRenderer>().color = Color.red;
                            crossHairScrp.followPositionUpdate(mouseWorldPositon);
                        }
                    }
                }
            }

            if (showCrossair)
                crossHairScrp.gameObject.SetActive(true);
            else
                crossHairScrp.gameObject.SetActive(false);
        }


        //start supportsystem
        if (targetObject != null && targetObject.tag == "Invader")
        {
            Invader targetInvaderScrp = targetObject.GetComponent<Invader>();

            if (SupportSystem.canSupport(targetInvaderScrp, activeWeapon) && targetInvaderScrp != this)
            {
                crossHairScrp.initFollowTargetBySupportSystem(targetObject.transform.GetChild(0).gameObject);

                switch (weapons[0].weaponType)
                {
                    case Weapon.WeaponType.Repair_drone:
                        if (!weapons[0].startSupportSystem(targetInvaderScrp, instantiatedCrossHair))
                        {
                            Destroy(instantiatedCrossHair.gameObject);
                        }
                        break;
                    case Weapon.WeaponType.Targeting_system:
                        if (weapons[0].startSupportSystem(targetInvaderScrp, instantiatedCrossHair))
                        {
                            Destroy(instantiatedCrossHair.gameObject);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (instantiatedCrossHair != null)
                    Destroy(instantiatedCrossHair);
            }
        }
        else
        {
            if (instantiatedCrossHair != null)
                Destroy(instantiatedCrossHair);
        }

        aimingDragCoroutineProcess = false;

        instantiatedCrossHair = null;
    }

    protected virtual GameObject getSupporterAimingDragTargetByPointerPosition(Vector3 mousePosition)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePosition, Vector2.zero);
        if (hit != null && hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                GameObject hitGO = hit[i].collider.gameObject;

                if (hitGO.tag == "Invader")
                {
                    switch (weapons[0].weaponType)
                    {
                        case Weapon.WeaponType.Repair_drone:
                            return hitGO.transform.parent.gameObject;
                        case Weapon.WeaponType.Targeting_system:
                            Invader invaderScrp = hitGO.transform.parent.GetComponent<Invader>();
                            if (invaderScrp.invaderClass == Invaderclass.Attacker || invaderScrp.invaderClass == Invaderclass.Boss)
                                return hitGO.transform.parent.gameObject;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return null;
    }

    #endregion


    #region Life methods
    public void repairing(int repairHitpoints)
    {
        if (currentHitpoints == hitpointsCurrentLevelDefault)
            return;

        currentHitpoints += repairHitpoints;
        if (currentHitpoints > hitpointsCurrentLevelDefault)
            currentHitpoints = hitpointsCurrentLevelDefault;

        removeDamageParticles();
    }

    private void removeDamageParticles()
    {
        Transform damageParticleParent = invaderTexture.transform.Find("DamageParticles");
        if (damageParticleParent == null || damageParticleParent.childCount <= 0)
            return;

        if (currentHitpoints >= hitpointsCurrentLevelDefault)
        {
            for (int i = 0; i < damageParticleParent.childCount; i++)
            {
                Destroy(damageParticleParent.GetChild(i).gameObject);
            }
        }
        else if (currentHitpoints < hitpointsCurrentLevelDefault)
        {
            Destroy(damageParticleParent.GetChild(Random.Range(0, damageParticleParent.childCount)).gameObject);
        }
    }

    /*
    * HP - damage
    * return bool: true = destroyed; false = only HP decreasing
    */
    public bool hit(int damageVal)
    {
        currentHitpoints -= damageVal;
        if (currentHitpoints <= 0)
        {
            currentHitpoints = 0;
            StatsHandler.instance.addTotalLostInvaders(1);
            destroyInvader();
            return true;
        }

        return false;
    }

    private void destroyInvader()
    {
        //drop powerup

        MyUtilities.instantiateAndDestroy(explosionEffectPrefab, transform.position, 6);

        StaticAudioHandler.playSound(SoundChooser.instance.explosion_Invader);

        if (instantiatedCrossHair != null)
            Destroy(instantiatedCrossHair.gameObject);

        IngameInvaderHandler.instance.removeInvaderFromList(gameObject);
    }

    #endregion


    #region unlock & upgrade methods
    public bool getUnlockConditionsFullfilled()
    {
        return unlockConditionsFullfilled;
    }

    public void checkUnlockConditionsFullfilled()
    {
        switch (name)
        {
            case "Hunter":
                unlockConditionsFullfilled = true;
                break;
            case "Bomber":
                if (StatsHandler.instance.hunterKills >= 50)
                    unlockConditionsFullfilled = true;
                break;
            case "Corvette":
                if (StatsHandler.instance.bomberKills >= 30)
                    unlockConditionsFullfilled = true;
                break;


            case "Scrap Crusher":
                if (StatsHandler.instance.TotalAsteroidKills >= 30)
                    unlockConditionsFullfilled = true;
                break;
            case "Defender":
                if (StatsHandler.instance.scrapCrusherDefences >= 100)
                    unlockConditionsFullfilled = true;
                break;
            case "Star Wall":
                if (StatsHandler.instance.defenderShieldUses >= 150)
                    unlockConditionsFullfilled = true;
                break;


            case "Scrap Collector":
                if (StatsHandler.instance.manualCollectedResPieces >= 1600)
                    unlockConditionsFullfilled = true;
                break;
            case "Engineer":
                if (StatsHandler.instance.TotalLostInvaders >= 160)
                    unlockConditionsFullfilled = true;
                break;
            case "AI-Supporter":
                if (FormationHandler.instance.unlockedFormationSlots >= 22)
                    unlockConditionsFullfilled = true;
                break;


            case "Destroyer":
                if (FormationHandler.instance.unlockedFormationSlots >= 25)
                    unlockConditionsFullfilled = true;
                break;
            case "Cruiser":
                if (StatsHandler.instance.destroyerKills >= 50)
                    unlockConditionsFullfilled = true;
                break;
            case "Behemoth":
                if (StatsHandler.instance.cruiserKills >= 50)
                    unlockConditionsFullfilled = true;
                break;

            default:
                break;
        }
    }

    public string getUnlockConditionsInfo()
    {
        switch (name)
        {
            case "Hunter":

                break;
            case "Bomber":
                return StatsHandler.instance.hunterKills + " / 50 kills with Hunter.";
            case "Corvette":
                return StatsHandler.instance.bomberKills + " / 30 kills with Bomber.";


            case "Scrap Crusher":
                return StatsHandler.instance.TotalAsteroidKills + " / 30 destroyed asteroids.";
            case "Defender":
                return StatsHandler.instance.scrapCrusherDefences + " / 100 defences with Scrap crusher.";
            case "Star Wall":
                return StatsHandler.instance.defenderShieldUses + " / 150 shield uses with Defender.";


            case "Scrap Collector":
                return StatsHandler.instance.manualCollectedResPieces + " / 1600 collected resource pieces by hand.";
            case "Engineer":
                return StatsHandler.instance.TotalLostInvaders + " / 160 ships losed.";
            case "AI-Supporter":
                return FormationHandler.instance.unlockedFormationSlots + " / 22 formation slots.";


            case "Destroyer":
                return FormationHandler.instance.unlockedFormationSlots + " / 25 formation slots.";
            case "Cruiser":
                return StatsHandler.instance.destroyerKills + " / 50 kills with Destroyer.";
            case "Behemoth":
                return "\n" + StatsHandler.instance.cruiserKills + " / 50 kills with Cruiser.";

            default:
                break;
        }

        return null;
    }

    public bool unlockInvader()
    {
        if (StatsHandler.instance.Resources >= unlockCost && getUnlockConditionsFullfilled())
        {
            StatsHandler.instance.subResources(unlockCost);
            unlocked = true;

            PopupHandler.instance.showShortInfoPopup("Unlocked!", PopupHandler.PopupAlignment.Top);

            AchievementsHolder.checkShipBuyAchivements();

            return true;
        }
        else
        {
            if (!getUnlockConditionsFullfilled())
            {
                PopupHandler.instance.showPopupWithOKButton("Unlock conditions:\n" + getUnlockConditionsInfo());
            }
            else
            {
                PopupHandler.instance.showPopupWithOKButton("Not enough Resources!");
            }
            return false;
        }
    }

    public bool upgrade()
    {
        if (StatsHandler.instance.Resources >= getUpgadeCost())
        {
            StatsHandler.instance.subResources(getUpgadeCost());

            level = getNextLevel();
            currentHitpoints = getHitpointsOfLevelN(level);

            if (weapons != null)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    weapons[i].upgrade(level);
                }
            }

            PopupHandler.instance.showShortInfoPopup("Level Up!", PopupHandler.PopupAlignment.Top);

            return true;
        }
        else
        {
            PopupHandler.instance.showPopupWithOKButton("Not enough Resources!");
            return false;
        }
    }

    public uint getUpgadeCost()
    {
        double calculatedUpgradeCost = upgradeCostLVL1;
        float tmpMultiplier = upgradeCostDefaultMultiplier;

        for (int currentLevelIndex = 2; currentLevelIndex < level + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, upgradeCostMultipliesOf))
            {
                tmpMultiplier = upgradeCostDefaultMultiplier * currentLevelIndex;
                calculatedUpgradeCost = calculatedUpgradeCost * 2;
            }
            else
            {
                tmpMultiplier = upgradeCostDefaultMultiplier;
                calculatedUpgradeCost = calculatedUpgradeCost + (calculatedUpgradeCost / 100 * tmpMultiplier);
            }

            //Debug.Log(currentLevelIndex + " | " + tmpMultiplier + " | " + upgradeCostMultipliesOf + " | " + calculatedUpgradeCost);
        }

        return (uint)System.Math.Round(calculatedUpgradeCost, 0);
    }

    public int getNextLevel()
    {
        return level + 1;
    }

    public int getHitpointsOfLevelN(int levelVal)
    {
        float defaultLVL1Val = hitpointsDefaultLVL1;
        float calculatedVal = hitpointsDefaultLVL1;
        int multipliesOf = hitpointsMultipliesOf;

        for (int currentLevelIndex = 1; currentLevelIndex < levelVal + 1; currentLevelIndex++)
        {
            if (MyUtilities.isMultipleOf(currentLevelIndex, multipliesOf))
            {
                calculatedVal = calculatedVal + defaultLVL1Val;
            }
        }
        //Debug.Log(level + " | " + multipliesOf + " | " + (int)System.Math.Round(calculatedVal, 0));

        return (int)System.Math.Round(calculatedVal, 0);
    }

    #endregion


    #region Save and Load methods

    public InvadersData save()
    {
        InvadersData data = new InvadersData();

        data.id = id;
        data.level = level;
        data.unlocked = unlocked;
        data.unlockConditionsFullfilled = unlockConditionsFullfilled;

        if (weapons != null)
        {
            WeaponData[] weaponDataList = new WeaponData[weapons.Length];

            for (int i = 0; i < weapons.Length; i++)
                weaponDataList[i] = weapons[i].save();

            data.weaponData = weaponDataList;
        }

        return data;
    }

    public void load()
    {
        InvadersData data = SaveLoadSystem.instance.loadInvader(id);

        level = data.level;
        currentHitpoints = getHitpointsOfLevelN(level);
        unlocked = data.unlocked;
        unlockConditionsFullfilled = data.unlockConditionsFullfilled;

        if (data.weaponData != null)
        {
            for (int i = 0; i < data.weaponData.Length; i++)
            {                
                weapons[i].load(data.weaponData[i], this);
            }
        }
    }

    #endregion

}