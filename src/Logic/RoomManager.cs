namespace WorldOfZuul.Logic;

public class RoomManager
{
    private readonly GameState World;
    public Room[,] Rooms { get; set; }

    public RoomManager(int width, int height, GameState _World)
    {
        Rooms = new Room[width, height];
        World = _World;
    }

    public void SetRoom(Room room, int x, int y)
    {
        Rooms[x, y] = room;
    }

    public Room? GetRoom(int x, int y)
    {
        if (x >= 0 && y >= 0 && 
            x < Rooms.GetLength(0) && y < Rooms.GetLength(1))
        {
            return Rooms[x, y];
        }

        return null;
    }

    public Room GetCurrentRoom()
    {
        return Rooms[World.Player.X, World.Player.Y];
    }

    // ! We don't move NPCs between rooms, we just have the same one in multiple rooms (e.g. old guy in townhall and his house)

    // public void MoveNpcToRoom(string npcId, int x, int y)
    // {
    //     var newRoom = GetRoom(x, y);

    //     if (newRoom != null && !newRoom.NPCs.Any(npc => npc.Id == npcId))
    //     {
    //         newRoom.NPCs.Add(NPCRegistry.CreateNPC(npcId));
            
    //         for (int roomX = 0; roomX < Rooms.GetLength(0); roomX++)
    //         {
    //             for (int roomY = 0; roomY < Rooms.GetLength(1); roomY++)
    //             {
    //                 if (Room)
    //             }
    //         }
    //     } else
    //     {
    //         Console.WriteLine("No Room found while trying to add an NPC!");
    //     }
    // }
}


