public class RoomManager
{
    private GameState world;
    public Room[,] Rooms { get; private set; }

    public RoomManager(int width, int height, GameState _world)
    {
        Rooms = new Room[width, height];
        world = _world;
    }

    public Room GetRoom(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Rooms.GetLength(0) && y < Rooms.GetLength(1))
        {
            return Rooms[x, y];
        }

        return null; // maybe I should throw an exception instead
    }

    public Room GetCurrentRoom()
    {
        return Rooms[world.PlayerX, world.PlayerY];
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
