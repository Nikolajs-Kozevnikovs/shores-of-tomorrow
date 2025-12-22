// using System.Runtime.CompilerServices;
// using WorldOfZuul.Logic;

// namespace UnitTests;

// public class RoomManagerTests
// {
//     [Test]
//     public void GetCurrentRoomOfThePlayer()
//     {
//         GameState world = new GameState(3, 3);
//         world.Player.X = 1;
//         world.Player.Y = 2;

//         Room testRoom = new Room(
//             tileIdentifier: 'T',
//             name: "Test Room",
//             description: "A test room",
//             background: "test.csv",
//             npcs: new List<string>(),
//             items: new List<string>(),
//             allowedItems: new List<string>()
//         );
        
//         world.RoomManager.SetRoom(testRoom, 1, 2);

//         Room? currentRoom = world.RoomManager.GetCurrentRoom();

//         Assert.That(currentRoom, Is.Not.Null, "Current room should not be null");
//         Assert.That(currentRoom, Is.EqualTo(testRoom), "Current room should be the test room at (1, 2)");
//     }
// }