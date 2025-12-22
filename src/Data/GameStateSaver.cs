namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;
using WorldOfZuul.Presentation;

public static class GameStateSaver
{
    const string SAVE_PATH = "./assets/saves/";
    private static JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public static void Save(GameState world, string directory)
    {
        string directory_path = $"{SAVE_PATH}/{directory}";


        if (!Directory.Exists(directory_path))
        {
            Directory.CreateDirectory(directory_path);
            File.Create(Path.Combine(directory_path, "rooms.json")).Dispose();
            File.Create(Path.Combine(directory_path, "player.json")).Dispose();
        } else
        {
            TUI.WriteLine("");
            TUI.WriteLine("Directory already exists! Are you sure you want to override the save (yes, no)?");
            Console.Write("> ");

            string? ans = Console.ReadLine();
            while (ans == null || (ans != "yes" && ans != "no"))
            {
                TUI.WriteLine("Wrong input");
                Console.Write("> ");
                ans = Console.ReadLine();
            }

            if (ans == "no")
            {
                TUI.WriteLine("Cancelling the save.");
                return;
            }

        }
        SaveRooms(world, directory);
        SavePlayer(world, directory);

        TUI.WriteLine("");
        TUI.WriteLine("Saved successfully!");
    }
    public static void SaveRooms(GameState world, string fileName)
    {
        List<RoomEntry> roomEntries = [];
        for (int i = 0; i < world.RoomManager.Rooms.GetLength(0); i++ )
        {
            for (int j = 0; j < world.RoomManager.Rooms.GetLength(1); j++ )
            {
                Room ?room = world.RoomManager.GetRoom(i, j);

                if (room != null && room.TileIdentifier != '-')
                {
                    roomEntries.Add(new RoomEntry(i, j, room));
                }
            }
        }

        string jsonString = JsonSerializer.Serialize(roomEntries, options);
        File.WriteAllText($"{SAVE_PATH}/{fileName}/rooms.json", jsonString);
    }
     

    public static void SavePlayer(GameState world, string fileName)
    {
        PlayerEntry playerEntry = new(
            x: world.Player.X,
            y: world.Player.Y,
            itemIds: world.Player.Inventory.Select(item => item.Id).ToList(),
            questProgression: world.Player.QuestProgression
        );
        string jsonString = JsonSerializer.Serialize(playerEntry, options);
        File.WriteAllText($"{SAVE_PATH}/{fileName}/player.json", jsonString);
    }
}