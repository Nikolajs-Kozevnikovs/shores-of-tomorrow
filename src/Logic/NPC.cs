namespace WorldOfZuul.Logic;
public class NPC
{
    public string Name { get; set; }
    public string Profession { get; set; }
    public int X { get; set; }  // Room grid coordinates
    public int Y { get; set; }
    public List<string> QuestsGiven { get; set; } = new List<string>();

    public NPC(string name, string profession, int x, int y, List<string> questsGiven)
    {
        Name = name;
        X = x;
        Y = y;
        QuestsGiven = questsGiven;
        Profession = profession;
    }
}