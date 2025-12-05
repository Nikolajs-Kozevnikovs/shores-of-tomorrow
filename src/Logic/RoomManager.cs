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

    public void SetRooms(List<RoomEntry> rooms)
    {
        foreach (RoomEntry room in rooms) {
            if (room.X >= 0 && room.Y >= 0 && 
                room.X < Rooms.GetLength(0) && room.Y < Rooms.GetLength(1))
            {
                World.RoomManager.Rooms[room.X, room.Y] = room.Room;
            }
        }
    }


    public Room? GetRoom(int x, int y)
    {
        if (x >= 0 && y >= 0 && 
            x < Rooms.GetLength(0) && y < Rooms.GetLength(1))
        {
            return Rooms[x, y];
        }

        return null; // maybe I should throw an exception instead
    }

    public Room GetCurrentRoom()
    {
        return Rooms[World.PlayerX, World.PlayerY];
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


public class RoomEntry
    {
        public int X {get; set; }
        public int Y {get; set; }
        public Room Room {get; set; }
        public RoomEntry(int x, int y, Room room)
        {
            X = x;
            Y = y;
            Room = room;
        }
    }