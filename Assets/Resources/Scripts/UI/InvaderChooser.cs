using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvaderChooser : MonoBehaviour
{
    public GameObject fadeInPanel;

    [Header("UI references")]
    public Button btAttacker;
    public Button btDefender;
    public Button btSupprter;
    public Button btBosses;
    public Color btSelectedColor;

    [Header("invader list item settings")]
    public GameObject invaderListItemPrefab;
    public Transform invaderItemListParent;
    private List<GameObject> instantiatedInvaderListItem;

    [Header("Drop & Drag invader settigs")]
    public Transform dragDropInvadersParent;

    public static InvaderChooser instance;



    private void Awake()
    {
        instance = this;
        fadeInPanel.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(loadInvaderListItemsCoroutine());

        btAttacker.onClick.AddListener(() => { showListOfInvaderClass(Invader.Invaderclass.Attacker); });
        btDefender.onClick.AddListener(() => { showListOfInvaderClass(Invader.Invaderclass.Defender); });
        btSupprter.onClick.AddListener(() => { showListOfInvaderClass(Invader.Invaderclass.Supporter); });
        btBosses.onClick.AddListener(() => { showListOfInvaderClass(Invader.Invaderclass.Boss); });
    }


    private IEnumerator loadInvaderListItemsCoroutine()
    {
        loadInvaderListItems();

        yield return new WaitForSeconds(0.01f);
        showListOfInvaderClass(Invader.Invaderclass.Attacker);

        yield return new WaitForSeconds(0.03f);
        fadeInPanel.SetActive(false);
    }

    private void loadInvaderListItems()
    {
        if (instantiatedInvaderListItem == null)
            instantiatedInvaderListItem = new List<GameObject>();

        List<GameObject> allInvaders = new List<GameObject>();
        allInvaders = MyUtilities.addListToList(allInvaders, InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Attacker));
        allInvaders = MyUtilities.addListToList(allInvaders, InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Defender));
        allInvaders = MyUtilities.addListToList(allInvaders, InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Supporter));
        allInvaders = MyUtilities.addListToList(allInvaders, InvaderLoader.instance.getLoadedInvadersByInvaderClass(Invader.Invaderclass.Boss));

        for (int invaderIndex = 0; invaderIndex < allInvaders.Count; invaderIndex++)
        {
            GameObject newInvaderItem = Instantiate(invaderListItemPrefab, invaderItemListParent);
            newInvaderItem.GetComponent<InvaderListItem>().init(allInvaders[invaderIndex].GetComponent<Invader>().id);
            instantiatedInvaderListItem.Add(newInvaderItem);
        }
    }

    private void showListOfInvaderClass(Invader.Invaderclass invaderClass)
    {
        unselectAllButtons();
        switch (invaderClass)
        {
            case Invader.Invaderclass.None:
                break;
            case Invader.Invaderclass.Attacker:
                MyUtilities.changeImageColor(btAttacker.gameObject, btSelectedColor);
                break;
            case Invader.Invaderclass.Defender:
                MyUtilities.changeImageColor(btDefender.gameObject, btSelectedColor);
                break;
            case Invader.Invaderclass.Boss:
                MyUtilities.changeImageColor(btBosses.gameObject, btSelectedColor);
                break;
            case Invader.Invaderclass.Supporter:
                MyUtilities.changeImageColor(btSupprter.gameObject, btSelectedColor);
                break;
            default:
                break;
        }

        for (int invaderIndex = 0; invaderIndex < instantiatedInvaderListItem.Count; invaderIndex++)
        {
            if (instantiatedInvaderListItem[invaderIndex].GetComponent<InvaderListItem>().invaderScrp.invaderClass == invaderClass)
                instantiatedInvaderListItem[invaderIndex].SetActive(true);
            else
                instantiatedInvaderListItem[invaderIndex].SetActive(false);
        }
    }

    private void unselectAllButtons()
    {
        MyUtilities.changeImageColor(btAttacker.gameObject, Color.black);
        MyUtilities.changeImageColor(btDefender.gameObject, Color.black);
        MyUtilities.changeImageColor(btSupprter.gameObject, Color.black);
        MyUtilities.changeImageColor(btBosses.gameObject, Color.black);
    }

    private void removeAllItems()
    {
        if (instantiatedInvaderListItem == null || instantiatedInvaderListItem.Count <= 0)
            return;

        for (int i = 0; i < instantiatedInvaderListItem.Count; i++)
        {
            Destroy(instantiatedInvaderListItem[i]);
        }
    }

    public void refreshList()
    {
        if (instantiatedInvaderListItem == null)
            return;

        for (int i = 0; i < instantiatedInvaderListItem.Count; i++)
        {
            instantiatedInvaderListItem[i].GetComponent<InvaderListItem>().refreshInfos();
        }
    }


    public void createDragDropInvader(Invader invaderScrp, FormationMenuSlotItem formationMenueSlotItemVal = null)
    {
        if (DragDropInvader.isDragging)
            return;

        GameObject newDragDropInvader = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Drag&DropInvader"), dragDropInvadersParent);
        newDragDropInvader.transform.position = MyUtilities.getMouseScreenPosition();
        newDragDropInvader.transform.GetChild(0).GetComponent<Image>().enabled = false;
        newDragDropInvader.name = invaderScrp.name + " (Drag&DropInvader)";

        DragDropInvader dragDropInvader = newDragDropInvader.GetComponent<DragDropInvader>();
        if (formationMenueSlotItemVal == null)
        {
            dragDropInvader.initByDrag(invaderScrp);
        }
        else
        {
            dragDropInvader.initInFormation(invaderScrp, formationMenueSlotItemVal);
        }
    }

    public void removeAllDragDropInvaders()
    {
        for (int i = 0; i < dragDropInvadersParent.childCount; i++)
        {
            dragDropInvadersParent.GetChild(i).GetComponent<DragDropInvader>().destroyDragAndDropInvader();
        }
    }

}
