
[System.Serializable]
public class InvaderFormationSlotSize
{
    public int row;
    public int column;
    public int[] formationSlotSize { get { return new int[] { row, column }; } }
    public int neededFormationSlots { get { return row * column; } }
}