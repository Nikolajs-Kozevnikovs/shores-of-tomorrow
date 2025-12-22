namespace WorldOfZuul.Data;

using WorldOfZuul.Logic;
public class PlayerEntry
{
    public int X {get; set;}
    public int Y {get; set;}
    public string ActiveQuestName { get; set; } = "";
    public List<string> ItemIds { get; set; } = new(); 

    public PlayerEntry() { }
    public PlayerEntry(int x, int y, string activeQuestName, List<string> itemIds) {
        X = x;
        Y = y;
        ActiveQuestName = activeQuestName;
        ItemIds = itemIds;
    }
}

public class RoomEntry
{
    public int X {get; set; }
    public int Y {get; set; }
    public Room Room { get; set; }
    public RoomEntry(int x, int y, Room room)
    {
        X = x;
        Y = y;
        Room = room;
    }
}
