namespace WorldOfZuul.Logic;
public class NPC
{
    public string Name { get; set; }
    public string Profession { get; set; }
    public int X { get; set; }  // Room grid coordinates
    public int Y { get; set; }
    public List<string> QuestsGiven { get; set; } = new List<string>();
    public List<string> QuestsFinishable { get; set; } = new List<string>();

    // Parameterless constructor for testing Move method
    public NPC()
    {
        Name = "";
        Profession = "";
    }

    //For testing Move method
    public NPC(string name, string profession, int x, int y)
    {
        Name = name;
        X = x;
        Y = y;
        Profession = profession;
    }

    public NPC(string name, string profession, int x, int y, List<string> questsGiven, List<string> questsFinishable)
    {
        Name = name;
        X = x;
        Y = y;
        QuestsGiven = questsGiven;
        QuestsFinishable = questsFinishable;
        Profession = profession;
    }
}