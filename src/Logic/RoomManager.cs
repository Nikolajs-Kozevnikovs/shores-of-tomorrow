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

    public void AddNpcToRoom(string npcId, int x, int y)
    {
        var room = GetRoom(x, y);
        if (room != null && !room.NPCs.Contains(npcId))
        {
            room.NPCs.Add(npcId);
        } else
        {
            Console.WriteLine("No Room found while trying to add an NPC!");
        }
    }

    public void RemoveNpcFromRoom(string npcId, int x, int y)
    {
        var room = GetRoom(x, y);
        room?.NPCs.Remove(npcId);
    }

    public void PlaceItem(string itemName, int x, int y)
    {
        var room = GetRoom(x, y);
        if (room != null)
        {
            room.Items.Add(itemName);
        }
    }
}


