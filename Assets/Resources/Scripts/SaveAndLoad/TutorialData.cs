
[System.Serializable]
public class TutorialData
{
    public bool firstTimeStartApplicationCompleted = false;
    public bool firstTimePlayIngameCompleted = false;

    public void setDefault()
    {
        firstTimeStartApplicationCompleted = false;
        firstTimePlayIngameCompleted = false;
    }
}
