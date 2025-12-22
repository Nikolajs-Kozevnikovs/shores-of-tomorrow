namespace WorldOfZuul.Logic;
public class NPC
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Profession { get; set; }
    public List<string> QuestsGiven { get; set; } = new();

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
        Id = id;
        Name = name;
        QuestsGiven = questsGiven;
        Profession = profession;
    }
}