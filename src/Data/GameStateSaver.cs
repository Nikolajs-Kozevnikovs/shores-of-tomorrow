// Does not support player data
namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;

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
            File.Create(Path.Combine(directory_path, "npcs.json")).Dispose();
            File.Create(Path.Combine(directory_path, "quests.json")).Dispose();
            File.Create(Path.Combine(directory_path, "items.json")).Dispose();
        } else
        {
            Console.WriteLine("Directory already exists! Are you sure you want to override the save (yes, no)?");
            Console.Write("> ");

            string? ans = Console.ReadLine();
            while (ans == null || (ans != "yes" && ans != "no"))
            {
                Console.WriteLine("Wrong input");
                Console.Write("> ");
                ans = Console.ReadLine();
            }

            if (ans == "no")
            {
                Console.WriteLine("Cancelling the save.");
                return;
            }

        }
        SaveRooms(world, directory);
        SaveNpcs(world, directory);
        SaveQuests(world, directory);
        SaveItems(world, directory);
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

    public static void SaveNpcs(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.NPCManager.NPCs, options);
        File.WriteAllText($"{SAVE_PATH}/{fileName}/npcs.json", jsonString);
    }

    public static void SaveQuests(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.QuestManager.Quests, options);
        File.WriteAllText($"{SAVE_PATH}/{fileName}/quests.json", jsonString);
    }

    public static void SaveItems(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.ItemManager.Items, options);
        File.WriteAllText($"{SAVE_PATH}/{fileName}/items.json", jsonString);
    }
     
}