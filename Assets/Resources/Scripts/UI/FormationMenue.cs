using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationMenue : MonoBehaviour
{
    public Button btMainMenue;
    public Text txtResources;

    public GameObject removeLine;

    [Header("Formation layout")]
    public Transform formationLayoutParent;
    public GameObject formationLayoutRowPrefab;
    public GameObject invaderSlotPrefab;
    public Button btRemoveFormation;

    [Header("Formation slot infos")]
    public Text txtPlaceCount;
    public Button btBuyFormationSlot;
    public Color canBuyColor;
    public Color cantBuyColor;
    public Color slotsFreeColor;
    public Color slotsFullColor;

    public List<FormationMenuSlotItem> instantiatedFormationMenuSlotItems { get; private set; }

    public static FormationMenue instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        btMainMenue.onClick.AddListener(() => { GameHandler.instance.showMainMenue(); });

        MyUtilities.instance.textCountAnimation(StatsHandler.instance.Resources, 0.25f, txtResources, false);
        setBtBuySlotInfos();

        btRemoveFormation.onClick.AddListener(() => { removeFormation(); });

        btBuyFormationSlot.onClick.AddListener(() => { buyFormationSlot(); });
        setBtBuySlotInfos();

        createFormationLayoutUI();
        refreshPlaceCountText();

        StartCoroutine(loadFormation());
    }


    public void showRemoveLine(bool show)
    {
        if(show)
            removeLine.SetActive(true);
        else
            removeLine.SetActive(false);
    }


    private void createFormationLayoutUI()
    {
        instantiatedFormationMenuSlotItems = new List<FormationMenuSlotItem>();

        for (int row = 0; row < FormationHandler.maxRows; row++)
        {
            GameObject newRow = Instantiate(formationLayoutRowPrefab, formationLayoutParent);
            newRow.name = "Row " + row;

            for (int column = 0; column < FormationHandler.maxColumns; column++)
            {
                GameObject newFormationMenuSlotItem = Instantiate(invaderSlotPrefab, newRow.transform);
                newFormationMenuSlotItem.name = "Slot [" + row + ", " + column + "]";
                newFormationMenuSlotItem.AddComponent<FormationMenuSlotItem>().init(new int[] { row, column });
                instantiatedFormationMenuSlotItems.Add(newFormationMenuSlotItem.GetComponent<FormationMenuSlotItem>());
            }
        }
    }

    private IEnumerator loadFormation()
    {
        yield return new WaitForSeconds(0.2f);

        for (int row = 0; row < FormationHandler.maxRows; row++)
        {
            for (int column = 0; column < FormationHandler.maxColumns; column++)
            {
                //create invader if formation[x,y] is an invader place 
                if (FormationHandler.instance.formation[row, column] != -1 && FormationHandler.instance.formation[row, column] != -2)
                {
                    FormationMenuSlotItem formationMenueSlotItem = getInstantiatedFormationMenuSlotItem(new int[] { row, column });
                    if (formationMenueSlotItem != null)
                    {
                        Invader invaderScrp = InvaderLoader.instance.getLoadedInvaderWithID(FormationHandler.instance.formation[row, column]).GetComponent<Invader>();
                        InvaderChooser.instance.createDragDropInvader(invaderScrp, formationMenueSlotItem);

                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        refreshPlaceCountText();
    }

    public void refreshFormationSlotColor()
    {
        Color newSlotColor = slotsFreeColor;
        if (FormationHandler.instance.freeFormationSlots <= 0)
            newSlotColor = slotsFullColor;

        for (int i = 0; i < instantiatedFormationMenuSlotItems.Count; i++)
        {
            instantiatedFormationMenuSlotItems[i].setColor(newSlotColor);
        }
    }

    private void removeFormation()
    {
        StaticAudioHandler.playSound(SoundChooser.instance.removeFormation);
        InvaderChooser.instance.removeAllDragDropInvaders();
        FormationHandler.instance.clearFormation();
        refreshPlaceCountText();
    }


    private void buyFormationSlot()
    {
        if (FormationHandler.instance.unlockedFormationSlots < 3)
        {
            PopupHandler.instance.showPopupWithOneButton("The first 3 slots are free!\n", "OK",
                                                        delegate ()
                                                        {
                                                            FormationHandler.instance.addFormationSlot();
                                                            FormationHandler.instance.addFormationSlot();
                                                            FormationHandler.instance.addFormationSlot();
                                                            refreshPlaceCountText();
                                                            PopupHandler.instance.showShortInfoPopup("Slots unlocked!", PopupHandler.PopupAlignment.Top);
                                                            setBtBuySlotInfos();
                                                        });
            return;
        }

        if (FormationHandler.instance.unlockedFormationSlots < FormationHandler.instance.maxFormationSlots)//Places to buy available. 
        {
            if (StatsHandler.instance.Resources >= FormationHandler.instance.formationSlotPrice)//Enough resource to buy new place.
            {
                StaticAudioHandler.playSound(SoundChooser.instance.buyFormationSlot);

                refreshTxtResources(StatsHandler.instance.Resources, FormationHandler.instance.formationSlotPrice);

                StatsHandler.instance.subResources(FormationHandler.instance.formationSlotPrice);
                FormationHandler.instance.addFormationSlot();
                refreshPlaceCountText();
                PopupHandler.instance.showShortInfoPopup("Slot purchased!", PopupHandler.PopupAlignment.Top);
                setBtBuySlotInfos();
                InvaderChooser.instance.refreshList();
            }
            else//Not enough resources to buy new place.
            {
                PopupHandler.instance.showPopupWithOKButton("Not enough Resources!");
            }
        }
        else//All formation slots bought.
        {
            PopupHandler.instance.showPopupWithOKButton("All formation slots bought!");
        }
    }

    private void setBtBuySlotInfos()
    {
        if (FormationHandler.instance.unlockedFormationSlots < FormationHandler.instance.maxFormationSlots)
        {
            if (StatsHandler.instance.Resources >= FormationHandler.instance.formationSlotPrice)
            {
                btBuyFormationSlot.GetComponent<Image>().color = canBuyColor;
            }
            else
            {
                btBuyFormationSlot.GetComponent<Image>().color = cantBuyColor;
            }

            btBuyFormationSlot.transform.GetChild(1).GetComponent<Text>().text = FormationHandler.instance.formationSlotPrice + " Res.";
            if (FormationHandler.instance.unlockedFormationSlots < 3)
            {
                btBuyFormationSlot.transform.GetChild(1).GetComponent<Text>().text = "Free";
                btBuyFormationSlot.GetComponent<Image>().color = canBuyColor;
            }

        }
        else
        {
            btBuyFormationSlot.gameObject.SetActive(false);
        }

    }

    private FormationMenuSlotItem getInstantiatedFormationMenuSlotItem(int[] formationPos)
    {
        for (int i = 0; i < instantiatedFormationMenuSlotItems.Count; i++)
        {
            if (instantiatedFormationMenuSlotItems[i].formationPosition[0] == formationPos[0] && instantiatedFormationMenuSlotItems[i].formationPosition[1] == formationPos[1])
            {
                return instantiatedFormationMenuSlotItems[i];
            }
        }

        return null;
    }


    public void refreshPlaceCountText()
    {
        txtPlaceCount.text = FormationHandler.instance.getOccupiedFormationSlots() + " / " + FormationHandler.instance.unlockedFormationSlots;
        refreshFormationSlotColor();
    }

    public void refreshTxtResources(uint oldRes, uint cost)
    {
        setBtBuySlotInfos();

        MyUtilities.instance.textSubCountAnimation(oldRes, cost, 0.25f, txtResources, false);
    }
}

public class FormationMenuSlotItem : MonoBehaviour
{
    public int[] formationPosition { get; private set; }


    public void init(int[] formationPositionVal)
    {
        formationPosition = formationPositionVal;
    }

    public void setColor(Color newColor)
    {
        transform.GetChild(0).GetComponent<Image>().color = newColor;
        transform.GetChild(0).GetComponent<Outline>().effectColor = newColor;
    }

}
