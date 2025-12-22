namespace WorldOfZuul.Logic;

using WorldOfZuul.Data;
using WorldOfZuul.Presentation;

public class GameState
{
    public RoomManager RoomManager { get; set; }
    public Player Player { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public GameState(int width, int height)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        RoomManager = new RoomManager(width, height, this);
        // load things from json
        GameStateLoader.LoadBase();
    }

    public void LoadData(string save_folder)
    {
        GameStateLoader.LoadDynamic(this, save_folder);
    }

    public void Save(string save_folder)
    {
        GameStateSaver.Save(this, save_folder);
    }

    public string[] GetSaves()
    {
        return GameStateLoader.GetSaves();
    }
}