[System.Serializable]
public class FormationData
{
    public int[,] invaderFormation; //[column, row]
    public int unlockedPlaces;

    public void setDefault()
    {
        invaderFormation = FormationHandler.createEmptyFormation();
        unlockedPlaces = 0;
    }
}
