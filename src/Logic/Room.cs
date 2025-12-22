namespace WorldOfZuul.Logic;

using System.Text.Json.Serialization;

public class Room : IItemContainer
{
    public char TileIdentifier {get; set; } = '-';
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Background {get; set; } = "assets/graphics/startScreen.csv"; // in case someone forgets to set up background there is a fallback image


    [JsonIgnore]
    public List<NPC> NPCs { get; set; } = new();

    [JsonPropertyName("npcs")]
    public List<string> NPCIds
    {
        get => NPCs.Select(npc => npc.Id).ToList();
        set => NPCs = (value ?? new List<string>())
            .Select(id => NPCRegistry.CreateNPC(id))
            .ToList();
    }


    [JsonIgnore] // ignore this when serializing
    public List<Item> Items { get; set; } = new();

    // temporary property for JSON load
    [JsonPropertyName("items")]
    public List<string> ItemIds
    {
        get => Items.Select(i => i.Id).ToList();
        set => Items = value.Select(id => ItemRegistry.CreateItem(id)).ToList();
    }


    public void AddItem(Item item)
    {
        Items.Add(item);
    }
    public bool RemoveItem(Item item)
    {
        return Items.Remove(item);
    }
    public bool IsInside(string itemId, int quantity)
    {
        int itemsInRoom = Items.Where(i => i.Id == itemId).ToList().Count;
        if (quantity == 0) {
            if (itemsInRoom == quantity)
            {
                return true;
            } else
            {
                return false;
            }
        }
        if (itemsInRoom > quantity)
        {
            return true;
        }
        return false;
    }


    public Room() 
    {
        // initialize lists
        Items = new List<Item>();
        NPCs = new List<NPC>();
        Background = "assets/graphics/startScreen.csv";
    }
}

