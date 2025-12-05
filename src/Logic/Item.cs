namespace WorldOfZuul.Logic;

public class Item
{
    public string Name { get; set; }
    public ItemLocation Location { get; set; }

    public Item(string name, ItemLocation location)
    {
        Name = name;
        Location = location;
    }
}

public class ItemLocation
{
    public int? RoomX { get; set; }
    public int? RoomY { get; set; }
    public bool IsHeldByPlayer { get; set; } = false;
    // public string ZoneId { get; set; };
}