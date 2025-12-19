namespace WorldOfZuul.Logic;

using WorldOfZuul.Data;
using WorldOfZuul.Presentation;

public class GameState
{
    public RoomManager RoomManager { get; set; }
    public NPCManager NPCManager { get; set; }
    public QuestManager QuestManager { get; set; }
    public ItemManager ItemManager { get; set; }
    public Player Player { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public GameState(int width, int height)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        RoomManager = new RoomManager(width, height, this);
        NPCManager = new NPCManager(this);
        QuestManager = new QuestManager(this);
        ItemManager = new ItemManager(this);
        // load things from json
        GameStateLoader.Load(this, "/new game");
        GameStateLoader.LoadPlayer(this, "/new game");
    }

    public void LoadData(string save_folder)
    {
        GameStateLoader.Load(this, save_folder);
        GameStateLoader.LoadPlayer(this, save_folder);
    }

    public void Save(string save_folder, TUI tui)
    {
        GameStateSaver.Save(this, save_folder, tui);
    }

    public string[] GetSaves()
    {
        return GameStateLoader.GetSaves();
    }
}