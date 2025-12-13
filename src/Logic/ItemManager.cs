namespace WorldOfZuul.Logic;
public class ItemManager
{
    private readonly GameState World;
    internal List<Item> Items { get; set; } = []; // <Name, Item>

    public ItemManager(GameState _World)
    {
        World = _World;
    }

    public void MoveToInventory(string itemName)
    {
        var items = Items.Where(x => x.Name == itemName).ToList();
        if (items.Count() == 0)
        {
            Console.WriteLine("No items matching the name found");
        }
        foreach (Item item in items)
        {
            if (!item.Location.IsHeldByPlayer && 
            item.Location.RoomX == World.Player.X && item.Location.RoomY == World.Player.Y)
            {
                item.Location.IsHeldByPlayer = true;
                item.Location.RoomX = null;
                item.Location.RoomY = null;
                return;
            }
        }
        Console.WriteLine("There is no such item in the room!");        
    }

    public void MoveOutOfInventory(string itemName)
    {
        var Item = Items.Find(x => x.Name == itemName);
        if (Item == null)
        {
            Console.WriteLine("No such item found");
            return;
        }

        var room = World.RoomManager.GetRoom(World.Player.X, World.Player.Y);
        if (room != null && !room.AllowedItems.Contains(itemName)) {  
            Console.WriteLine("You cannot put this item here!");
            return;
        }
        
        Item.Location.IsHeldByPlayer = false;
        Item.Location.RoomX = World.Player.X;
        Item.Location.RoomY = World.Player.Y;
        World.RoomManager.PlaceItem(Item.Name, World.Player.X, World.Player.Y);
    }

    public List<Item> GetPlayerInventory()
    {
        return Items.Where(item => item.Location.IsHeldByPlayer).ToList();
    }
}