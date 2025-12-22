namespace WorldOfZuul.Logic;

public class Player : IItemContainer
{
    private readonly GameState World; 
    // movement
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int[]? PreviousCoords { get; set; }
    public Room CurrentRoom { get; set; }
    // items
    public List<Item> Inventory { get; set; } 
    List<Item> IItemContainer.Items => Inventory;
    // quest progression
    public QuestProgression QuestProgression {get; set; } = new();

    public Player(int x, int y, QuestProgression questProgression, List<Item> inventory, GameState world)
    {
        X = x;
        Y = y;
        World = world;
        Room? room = World.RoomManager.GetRoom(x, y) ?? throw new Exception("No room at starting coordinates! Breaking.");
        CurrentRoom = room;
        QuestProgression = questProgression;
        Inventory = inventory;
    }

    public bool RemoveItem(string itemId, int count)
    {
        var items = Inventory.Where(i => i.Id == itemId).Take(count).ToList();
        if (items.Count < count)
            return false;

        foreach (var item in items)
            Inventory.Remove(item);

        return true;
    }
    public void RemoveItem(Item item)
    {
        Inventory.Remove(item);
    }

    // inside Player.cs
    public void AddItem(Item item)
    {
        Inventory.Add(item);
    }

    public bool IsInside(string itemId) => ((IItemContainer)this).IsInside(itemId);

    public string? Move(string direction, string? amount)
    {
        int count = 1;
        if (amount != null)
        {
            try
            {
                count = int.Parse(amount);
            } catch (Exception) { }
        }
        int newX = X;
        int newY = Y;

        switch (direction)
        {
            case "north":
                newY -= count;
                break;
            case "east":
                newX += count;
                break;
            case "south":
                newY += count;
                break;
            case "west":
                newX -= count;
                break;
        }

        Room? target = World.RoomManager.GetRoom(newX, newY);
        if (target == null || target.TileIdentifier == '-')
        {
            return "You can't go that way!";
        }

        PreviousCoords = [X, Y];
        X = newX;
        Y = newY;

        CurrentRoom = World.RoomManager.GetCurrentRoom();
        return null;
    }

    public string? Back()
    {
        if (PreviousCoords == null)
        {
            return "You can't go back from here!";

        }

        X = PreviousCoords[0];
        Y = PreviousCoords[1];

        PreviousCoords = null;

        CurrentRoom = World.RoomManager.GetCurrentRoom();
        return null;
    }
}
