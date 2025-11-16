public class Room
{
    public char TileIdentifier {get; set; } = '-';
    public string Name { get; set; }
    public string Description { get; set; }
    public string Background {get; set; } = "assets/graphics/startScreen.csv"; // in case someone forgets to set up background there is a fallback image
    public List<string> NPCs { get; set; } = [];
    public List<string> Items { get; set; } = [];
    public List<string> AllowedItems { get; set; } = [];

    // public List<Zone> Zones { get; set; } = new List<Zone>();
}