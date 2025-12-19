using Spectre.Console;

namespace WorldOfZuul.Logic;

public class Player : IItemContainer
{
    private readonly GameState World; 
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int[]? PreviousCoords { get; set; }
    public Room CurrentRoom { get; set; }
    public string ActiveQuestName { get; set; } = "";
    // item management
    public List<Item> Inventory { get; set; } 
    List<Item> IItemContainer.Items => Inventory;

    public Player(int x, int y, string activeQuestName, List<Item> inventory, GameState world)
    {
        X = x;
        Y = y;
        World = world;
        Room? room = World.RoomManager.GetRoom(x, y) ?? throw new Exception("No room at starting coordinates! Breaking.");
        CurrentRoom = room;
        Inventory = inventory;
        ActiveQuestName = activeQuestName;
    }

    public void AddItem(Item item) => ((IItemContainer)this).AddItem(item);
    public bool RemoveItem(Item item) => ((IItemContainer)this).RemoveItem(item);
    public bool IsInside(string itemId) => ((IItemContainer)this).IsInside(itemId);
    
    public string? Move(string direction)
    {
        int newX = X;
        int newY = Y;

        switch (direction)
        {
            case "south":
                newX++;
                break;
            case "north":
                newX--;
                break;
            case "west":
                newY--;
                break;
            case "east":
                newY++;
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