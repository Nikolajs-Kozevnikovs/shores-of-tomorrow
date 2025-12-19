namespace WorldOfZuul.Logic;
public class NPC
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Profession { get; set; }
    public List<string> QuestsGiven { get; set; } = new();

    public NPC(string id, string name, string profession, List<string> questsGiven)
    {
        Id = id;
        Name = name;
        QuestsGiven = questsGiven;
        Profession = profession;
    }
}