public class ItemManager
{
    private GameState world { get ; }
    public List<Item> Items {get; set; } = []; // <Name, Item>

    public ItemManager(GameState _world)
    {
        world = _world;
    }

    public void MoveToInventory(string itemName)
    {
        var Item = Items.Find(x => x.Name == itemName);
        if (Item == null)
        {
            Console.WriteLine("No such item found");
            return;
        }

        Item.Location.IsHeldByPlayer = true;
        Item.Location.RoomX = null;
        Item.Location.RoomY = null;
    }

    public void MoveOutOfInventory(string itemName)
    {
        var Item = Items.Find(x => x.Name == itemName);
        if (Item == null)
        {
            Console.WriteLine("No such item found");
            return;
        }

        var Room = world.RoomManager.GetRoom(world.PlayerX, world.PlayerY);
        if (!Room.AllowedItems.Contains(itemName)) {  
            Console.WriteLine("You cannot put this item here!");
            return;
        }
        
        Item.Location.IsHeldByPlayer = false;
        Item.Location.RoomX = world.PlayerX;
        Item.Location.RoomY = world.PlayerY;
        world.RoomManager.PlaceItem(Item.Name, world.PlayerX, world.PlayerY);
    }

    public List<Item> GetPlayerInventory()
    {
        return Items.Where(item => item.Location.IsHeldByPlayer).ToList();
    }
}