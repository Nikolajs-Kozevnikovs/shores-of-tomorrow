namespace WorldOfZuul.Data;

using WorldOfZuul.Logic;
public class PlayerEntry
{
    public int X {get; set;}
    public int Y {get; set;}
    public List<string> ItemIds { get; set; } = new(); 
    public QuestProgression QuestProgression { get; set; } = new();

    public PlayerEntry() { }
    public PlayerEntry(int x, int y, List<string> itemIds, QuestProgression questProgression) {
        X = x;
        Y = y;
        ItemIds = itemIds;
        QuestProgression = questProgression;
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
