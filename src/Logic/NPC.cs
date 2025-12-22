namespace WorldOfZuul.Logic;
public class NPC
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Profession { get; set; }

    public NPC(string id, string name, string profession)
    {
        Id = id;
        Name = name;
        Profession = profession;
    }
}