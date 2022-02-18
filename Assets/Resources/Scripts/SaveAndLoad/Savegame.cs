[System.Serializable]
public class Savegame
{
    public string savegameVersion;
    public bool soundIsOn = true;
    public bool musicIsOn = true;
    public bool showFPS = false;

    public TutorialData tutorialData;
    public StatsData statsData;
    public FormationData formationData;
    public InvadersData[] invadersData;
}
