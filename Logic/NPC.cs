public class NPC
{
    public string Name { get; set; }
    public int X { get; set; }  // Room grid coordinates
    public int Y { get; set; }
    public List<string> QuestsGiven { get; set; } = new List<string>();
    public List<string> QuestsFinishable { get; set; } = new List<string>();
}