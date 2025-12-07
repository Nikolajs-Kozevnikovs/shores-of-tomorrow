namespace WorldOfZuul.Logic;

using WorldOfZuul.Data;
public class GameState
{
    public RoomManager RoomManager { get; set; }
    public NPCManager NPCManager { get; set; }
    public QuestManager QuestManager { get; set; }
    public ItemManager ItemManager { get; set; }
    public Player Player { get; set; }
    

    public GameState(int width, int height)
    {
        RoomManager = new RoomManager(width, height, this);
        NPCManager = new NPCManager(this);
        QuestManager = new QuestManager(this);
        ItemManager = new ItemManager(this);
        
        // load things from json
        GameStateLoader.Load(this, "/");

        Player = new Player(0, 0, this);
    }
}