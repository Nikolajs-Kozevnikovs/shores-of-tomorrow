// using System.Reflection;
// using WorldOfZuul.Logic;

// namespace UnitTests;

// public class NPCManagerTests
// {
//     [Test]
//     public void MoveNPCToDifferentRoom()
//     {
//         GameState world = new GameState(3, 3);
//         world.Player.X = 1;
//         world.Player.Y = 2;

//         Room room = new Room(
//             tileIdentifier: 'T',
//             name: "Test Room",
//             description: "A test room",
//             background: "test.csv",
//             npcs: new List<string>(),
//             items: new List<string>(),
//             allowedItems: new List<string> { "Key" }
//         );
//         world.RoomManager.SetRoom(room, 1, 2);
        
//         world.NPCManager.NPCs.Clear();
        
//         NPC npc = new NPC("TestNPC", "TestProfession", 0, 0);
//         world.NPCManager.NPCs.Add("TestNPC", npc);

//         world.NPCManager.MoveNPC("TestNPC", 1, 2);

//         Assert.That(npc.X, Is.EqualTo(1), "NPC X coordinate should be updated");
//         Assert.That(npc.Y, Is.EqualTo(2), "NPC Y coordinate should be updated");
//     }
// }

