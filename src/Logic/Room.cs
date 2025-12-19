namespace WorldOfZuul.Logic;

using System.Text.Json.Serialization;

public class Room
{
    public char TileIdentifier {get; set; } = '-';
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Background {get; set; } = "assets/graphics/startScreen.csv"; // in case someone forgets to set up background there is a fallback image
    public List<string> NPCs { get; set; } = [];

    [JsonIgnore] // ignore this when serializing
    public List<Item> Items { get; set; } = new();

    // temporary property for JSON load
    [JsonPropertyName("items")]
    public List<string> ItemIds
    {
        get => Items.Select(i => i.Id).ToList();
        set => Items = value.Select(id => ItemRegistry.CreateItem(id)).ToList();
    }
    public void AddItem(Item item) => ((IItemContainer)this).AddItem(item);
    public bool RemoveItem(Item item) => ((IItemContainer)this).RemoveItem(item);
    public bool IsInside(string itemId) => ((IItemContainer)this).IsInside(itemId);

    public Room() 
    {
        // initialize lists
        Items = new List<Item>();
        NPCs = new List<string>();
        TileIdentifier = '-';
        Background = "assets/graphics/startScreen.csv";
    }


    public Room(char tileIdentifier, string name, string description, string background, List<string> npcs, List<Item> items)
    {
        TileIdentifier = tileIdentifier;
        Name = name;
        Description = description;
        Background = background;
        NPCs = npcs;
        Items = items;
    }

    // public List<Zone> Zones { get; set; } = new List<Zone>();
}

