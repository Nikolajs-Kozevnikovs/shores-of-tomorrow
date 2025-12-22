// namespace WorldOfZuul.Logic;
// public class NPCManager
// {
//     private readonly GameState World;
//     public Dictionary<string, NPC> NPCs { get; set; } = [];

//     public NPCManager(GameState _World)
//     {
//         World = _World;
//     }


//     public void MoveNPC(string npcName, int newX, int newY)
//     {
// #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//         if (NPCs.TryGetValue(npcName, out NPC npc))
//         {
//             World.RoomManager.RemoveNpcFromRoom(npcName, npc.X, npc.Y);
//             npc.X = newX;
//             npc.Y = newY;
//             World.RoomManager.AddNpcToRoom(npcName, newX, newY);
//         }
// #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
//     }

//     public NPC GetNPC(string npcName)
//     {
//         NPC? npc = NPCs[npcName];
        
//         if (npc == null)
//         {
//             throw new Exception("Error occured when trying to find an npc!");
//         }

//         return npc;
//     }
// }
