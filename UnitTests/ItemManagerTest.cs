
using WorldOfZuul.Logic;

namespace UnitTests;

public class ItemManagerTests
{
    [Test]
    public void MoveKeyOutFromInventory_MovesKeyToCurrentRoom()
    {
        GameState world = new GameState(3, 3);
        world.Player.X = 1;
        world.Player.Y = 2;

        Room room = new Room(
            tileIdentifier: 'T',
            name: "Test Room",
            description: "A test room",
            background: "test.csv",
            npcs: new List<string>(),
            items: new List<string>(),
            allowedItems: new List<string> { "Key" }
        );
        world.RoomManager.SetRoom(room, 1, 2);

        // Access internal Items list via reflection 
        var itemsField = typeof(ItemManager).GetProperty("Items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var items = (List<Item>)itemsField!.GetValue(world.ItemManager)!;
        
        items.Clear();
        
        Item key = new Item("Key", new ItemLocation {
            IsHeldByPlayer = true,
            RoomX = null,
            RoomY = null
        });

        items.Add(key);

        Room? testRoom = world.RoomManager.GetRoom(1, 2);
        Assert.That(testRoom, Is.Not.Null, "Room should exist at (1,2)");
        Assert.That(testRoom!.AllowedItems.Contains("Key"), Is.True, "Room should allow Key items");

        world.ItemManager.MoveOutOfInventory("Key");

        Assert.That(key.Location.IsHeldByPlayer, Is.False, "Key should no longer be held by player");
        Assert.That(key.Location.RoomX, Is.EqualTo(1));
        Assert.That(key.Location.RoomY, Is.EqualTo(2));
    }
}
