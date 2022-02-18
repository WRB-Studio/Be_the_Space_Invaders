using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvaderLoader : MonoBehaviour
{
    private List<GameObject> loadedInvaders;


    public static InvaderLoader instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void init()
    {
        transform.position = new Vector3(100, 100);//prevent to view loaded invaders on screen

        instance.loadAllInvaders();
    }

    private void loadAllInvaders()
    {
        if (loadedInvaders == null)
        {
            loadedInvaders = new List<GameObject>();

            List<GameObject> allInvaders = getAllInvaderPrefabs();

            for (int invaderIndex = 0; invaderIndex < allInvaders.Count; invaderIndex++)
            {
                GameObject newInvader = Instantiate(allInvaders[invaderIndex], transform);
                newInvader.name = newInvader.name.Replace("(Clone)", "");
                loadedInvaders.Add(newInvader);
                newInvader.GetComponent<Invader>().init(true);
            }
        }
    }


    public List<GameObject> getAllLoadedInvaders()
    {
        List<GameObject> allInvaders = getLoadedInvadersByInvaderClass(Invader.Invaderclass.Attacker);
        MyUtilities.addListToList(allInvaders, getLoadedInvadersByInvaderClass(Invader.Invaderclass.Defender));
        MyUtilities.addListToList(allInvaders, getLoadedInvadersByInvaderClass(Invader.Invaderclass.Supporter));
        MyUtilities.addListToList(allInvaders, getLoadedInvadersByInvaderClass(Invader.Invaderclass.Boss));

        return allInvaders.OrderBy(o => o.GetComponent<Invader>().id).ToList();//sort invader by invader id
    }

    public GameObject getLoadedInvaderWithID(int id)
    {
        for (int invaderIndex = 0; invaderIndex < loadedInvaders.Count; invaderIndex++)
        {
            if (loadedInvaders[invaderIndex].GetComponent<Invader>().id == id)
            {
                return loadedInvaders[invaderIndex];
            }
        }

        return null;
    }

    public GameObject getLoadedInvaderWithName(string nameVal)
    {
        for (int invaderIndex = 0; invaderIndex < loadedInvaders.Count; invaderIndex++)
        {
            if (loadedInvaders[invaderIndex].GetComponent<Invader>().name == nameVal)
            {
                return loadedInvaders[invaderIndex];
            }
        }

        return null;
    }


    public List<GameObject> getLoadedInvadersByInvaderClass(Invader.Invaderclass invaderClass)
    {
        List<GameObject> filteredInvaderList = new List<GameObject>();
        for (int i = 0; i < loadedInvaders.Count; i++)
        {
            if (loadedInvaders[i].GetComponent<Invader>().invaderClass == invaderClass)
            {
                filteredInvaderList.Add(loadedInvaders[i]);
            }
        }

        return filteredInvaderList.OrderBy(o => o.GetComponent<Invader>().id).ToList();//sort invader by invader id
    }

    public List<GameObject> getLoadedUnlockedInvadersWithMissleLauncher()
    {
        List<GameObject> invadersWithMissleLauncher = null;
        for (int i = 0; i < loadedInvaders.Count; i++)
        {
            if (loadedInvaders[i].GetComponent<Invader>().unlocked && loadedInvaders[i].GetComponent<Invader>().getWeaponsOfType(Weapon.WeaponType.Missle_launcher).Length > 0)
            {
                if (invadersWithMissleLauncher == null)
                    invadersWithMissleLauncher = new List<GameObject>();
                invadersWithMissleLauncher.Add(loadedInvaders[i]);
            }
        }

        return invadersWithMissleLauncher;
    }

    public List<GameObject> getLoadedUnlockedInvadersWithBomb()
    {
        List<GameObject> invadersWithBomb = null;
        for (int i = 0; i < loadedInvaders.Count; i++)
        {
            if (loadedInvaders[i].GetComponent<Invader>().unlocked && loadedInvaders[i].GetComponent<Invader>().getWeaponsOfType(Weapon.WeaponType.Bomb).Length > 0)
            {
                if (invadersWithBomb == null)
                    invadersWithBomb = new List<GameObject>();
                invadersWithBomb.Add(loadedInvaders[i]);
            }
        }

        return invadersWithBomb;
    }


    //------------------static methods----------------------

    public static List<GameObject> getAllInvaderPrefabs()
    {
        List<GameObject> allInvaderPrefabs = new List<GameObject>();
        allInvaderPrefabs = MyUtilities.addListToList(allInvaderPrefabs, Resources.LoadAll<GameObject>("Prefabs/Invaders/Attackers").ToList());
        allInvaderPrefabs = MyUtilities.addListToList(allInvaderPrefabs, Resources.LoadAll<GameObject>("Prefabs/Invaders/Defenders").ToList());
        allInvaderPrefabs = MyUtilities.addListToList(allInvaderPrefabs, Resources.LoadAll<GameObject>("Prefabs/Invaders/Supporters").ToList());
        allInvaderPrefabs = MyUtilities.addListToList(allInvaderPrefabs, Resources.LoadAll<GameObject>("Prefabs/Invaders/Boss").ToList());

        return allInvaderPrefabs;
    }

    public static List<GameObject> getAllInvaderAttackersPrefabs()
    {
        List<GameObject> newList = new List<GameObject>();
        newList = MyUtilities.addListToList(newList, Resources.LoadAll<GameObject>("Prefabs/Invaders/Attackers").ToList());
        return newList;
    }

    public static List<GameObject> getAllInvaderDefendersPrefabs()
    {
        List<GameObject> newList = new List<GameObject>();
        newList = MyUtilities.addListToList(newList, Resources.LoadAll<GameObject>("Prefabs/Invaders/Defenders").ToList());
        return newList;
    }

    public static List<GameObject> getAllInvaderSupportersPrefabs()
    {
        List<GameObject> newList = new List<GameObject>();
        newList = MyUtilities.addListToList(newList, Resources.LoadAll<GameObject>("Prefabs/Invaders/Supporters").ToList());
        return newList;
    }

    public static List<GameObject> getAllInvaderBossPrefabs()
    {
        List<GameObject> newList = new List<GameObject>();
        newList = MyUtilities.addListToList(newList, Resources.LoadAll<GameObject>("Prefabs/Invaders/Boss").ToList());
        return newList;
    }


    public static GameObject getInvaderPrefabWithID(int id)
    {
        List<GameObject> allInvaders = getAllInvaderPrefabs();
        for (int invaderIndex = 0; invaderIndex < allInvaders.Count; invaderIndex++)
        {
            if (allInvaders[invaderIndex].GetComponent<Invader>().id == id)
                return allInvaders[invaderIndex];
        }

        return null;
    }

}
