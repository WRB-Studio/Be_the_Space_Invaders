using UnityEngine;

public class FormationHandler : MonoBehaviour
{
    private static int max_Rows = 5;
    private static int max_Columns = 8;
    private static int[,] invader_formation; //[row, column]; -1 = no invader; -2 = locked placed by some invader that slot size > [1,1]; >= 0 = specific invader.id
    private static int unlocked_Formation_Slots = 0;
    private static int min_Formation_Sots = 3;

    public static int maxRows { get { return max_Rows; } }
    public static int maxColumns { get { return max_Columns; } }
    public int[,] formation
    {
        get
        {
            if (invader_formation == null)
            {
                invader_formation = new int[maxRows, maxColumns];
                for (int row = 0; row < max_Rows; row++)
                {
                    for (int column = 0; column < max_Columns; column++)
                    {
                        invader_formation[row, column] = -1;
                    }
                }
            }

            return invader_formation;
        }
    }
    public int unlockedFormationSlots { get { return unlocked_Formation_Slots; } }
    public int minFormationSlots { get { return min_Formation_Sots; } }
    public int maxFormationSlots { get { return maxRows * maxColumns; } }
    public uint formationSlotPrice
    {
        get
        {
            float startVal = 7;
            int increaseByMultipliesOf = 10;
            float currentMultiplierAdd = 1.0f;
            float extraCostMultiplier = 1.85f;
            float calculatedVal = startVal;

            for (int currentSlotIndex = 2; currentSlotIndex <= unlocked_Formation_Slots; currentSlotIndex++)
            {
                if (MyUtilities.isMultipleOf(currentSlotIndex, increaseByMultipliesOf))
                {
                    currentMultiplierAdd = currentMultiplierAdd * (increaseByMultipliesOf / extraCostMultiplier);
                }
                else
                {
                    currentMultiplierAdd = currentMultiplierAdd + 1;
                }

                calculatedVal = startVal * currentSlotIndex * currentMultiplierAdd;

                //Debug.Log(currentSlotIndex + " | " + multiplier + " | " + multipliesOf + " | " + (int)System.Math.Round(calculatedVal, 0));
            }

            return (uint)System.Math.Round(calculatedVal, 0);
        }
    }
    public int freeFormationSlots { get { return unlockedFormationSlots - getOccupiedFormationSlots(); } }


    public static FormationHandler instance;
    


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void init()
    {
        if (invader_formation == null)
            createEmptyFormation();

        load();
    }

    public static int[,] createEmptyFormation()
    {
        invader_formation = new int[maxRows, maxColumns];
        for (int row = 0; row < max_Rows; row++)
        {
            for (int column = 0; column < max_Columns; column++)
            {
                invader_formation[row, column] = -1;
            }
        }

        return invader_formation;
    }

    public void clearFormation()
    {
        createEmptyFormation();
    }

    public bool isFormationEmpty()
    {
        if (getOccupiedFormationSlots() == 0)
            return true;
        return false;
    }

    public int getOccupiedFormationSlots()
    {
        int counter = 0;
        for (int row = 0; row < max_Rows; row++)
        {
            for (int column = 0; column < max_Columns; column++)
            {
                if (invader_formation[row, column] != -1)
                {
                    counter++;
                }
            }
        }
        return counter;
    }


    public int getSettedInvadersById(int id)
    {
        int count = 0;

        for (int row = 0; row < max_Rows; row++)
        {
            for (int column = 0; column < max_Columns; column++)
            {
                if (invader_formation[row, column] == id)
                {
                    count++;
                }
            }
        }

        return count;
    }


    public bool addFormationSlot()
    {
        if (unlocked_Formation_Slots + 1 > maxFormationSlots)
            return false;

        unlocked_Formation_Slots++;

        return true;
    }

    public bool setInvaderOnFormationSlot(int id, int[] place)
    {
        Invader invaderScrp = InvaderLoader.instance.getLoadedInvaderWithID(id).GetComponent<Invader>();
        int[] invaderFormationSlotSize = invaderScrp.formationSlotSize.formationSlotSize;

        if (freeFormationSlots >= invaderFormationSlotSize[0] * invaderFormationSlotSize[1]) //Check if there are enough free formation slots.
        {
            if (!invaderFitsInFormation(invaderScrp, place))
            {
                PopupHandler.instance.showPopupWithOKButton("Can't set invader to formation slot!");
                return false;
            }

            //place invader in to formation
            for (int row = 0; row < invaderFormationSlotSize[0]; row++)
            {
                for (int column = 0; column < invaderFormationSlotSize[1]; column++)
                {
                    formation[place[0] + row, place[1] + column] = -2;
                }
            }
            formation[place[0], place[1]] = id;

            return true;
        }
        else
        {
            PopupHandler.instance.showPopupWithOKButton("Not enough free formation slots.");
            return false;
        }
    }

    public bool invaderFitsInFormation(Invader invaderScrp, int[] place)
    {
        int[] invaderFormationSlotSize = invaderScrp.formationSlotSize.formationSlotSize;

        for (int slotSizeRowIndex = 0; slotSizeRowIndex < invaderFormationSlotSize[0]; slotSizeRowIndex++)
        {
            for (int slotSizeColumnIndex = 0; slotSizeColumnIndex < invaderFormationSlotSize[1]; slotSizeColumnIndex++)
            {
                /*Debug.Log("Place = [" + place[0] + "," + place[1] + "]\n" +
                          "Size = [" + invaderFormationSlotSize[0] + "," + invaderFormationSlotSize[1] + "]\n" +
                          "Current SizeIndex = [" + slotSizeRowIndex + "," + slotSizeColumnIndex + "]\n" +
                          "Checking formation place = [" + (place[0] + slotSizeRowIndex) + "," + (place[1] + slotSizeColumnIndex) + "]\n" + 
                          "MaxRange = ["+ maxRows  + "," + maxRows + "]");*/
                if ((place[0] + slotSizeRowIndex) >= maxRows || (place[1] + slotSizeColumnIndex) >= maxColumns) //check place x,y is in formation array range
                    return false;
                else if (formation[(place[0] + slotSizeRowIndex), (place[1] + slotSizeColumnIndex)] != -1) //check no other invader is on place x,y 
                    return false;
            }
        }

        return true;
    }

    public bool removeInvaderFromPlace(int[] place)
    {
        int[] invaderFormationSlotSize;

        //if formation place id = -1 Remove invader
        if (formation[place[0], place[1]] != -1)
        {
            invaderFormationSlotSize = InvaderLoader.instance.getLoadedInvaderWithID(formation[place[0], place[1]]).GetComponent<Invader>().formationSlotSize.formationSlotSize;
            for (int row = 0; row < invaderFormationSlotSize[0]; row++)
            {
                for (int column = 0; column < invaderFormationSlotSize[1]; column++)
                {
                    formation[place[0] + row, place[1] + column] = -1;
                }
            }
            return true;
        }

        return false;
    }

    public bool formationSlotIsFree(int[] place)
    {
        if (formation[place[0], place[1]] > -1)
            return true;

        return false;
    }


    public void debugShowAllInvaderPositonInFormation()
    {
        string invaderPositions = "";
        //show formationposition of invaders in Formation
        for (int row = 0; row < maxRows; row++)
        {
            for (int column = 0; column < maxColumns; column++)
            {
                if (formation[row, column] != -1)
                    invaderPositions += "[" + row + ", " + column + "]\n";
            }
        }
        Debug.Log(invaderPositions);
    }


    /*------------Save and Load methods---------------*/

    public FormationData save()
    {
        FormationData data = new FormationData();

        data.invaderFormation = invader_formation;
        data.unlockedPlaces = unlocked_Formation_Slots;

        return data;
    }

    public void load()
    {
        FormationData data = SaveLoadSystem.instance.loadFormation();

        if (!GameHandler.staticFormationWipe)
            invader_formation = data.invaderFormation;
        else
            createEmptyFormation();

        unlocked_Formation_Slots = data.unlockedPlaces;
    }
}
