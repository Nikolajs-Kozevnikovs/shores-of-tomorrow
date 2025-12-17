namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;

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

public static class GameStateLoader
{
    const string SAVE_PATH = "./assets/saves/";

    public static string[] GetSaves()
    {
        string[] folders = Directory.GetDirectories(SAVE_PATH).Select(path => Path.GetFileName(path).ToLower()).ToArray();
        return folders;
    }

    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static void Load(GameState world, string directory)
    {
        LoadRooms(world, Path.Combine(directory, "rooms.json"));
        LoadNpcs(world, Path.Combine(directory, "npcs.json"));
        LoadQuests(world, Path.Combine(directory, "quests.json"));
        LoadItems(world, Path.Combine(directory, "items.json"));
        // LoadPlayer(world, Path.Combine(directory, "player.json"));
    }


    private static void LoadRooms(GameState world, string fileName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{fileName}");
        var rooms = JsonSerializer.Deserialize<List<RoomEntry>>(json, options);

        if (rooms != null)
        {
            SetRooms(world, rooms);
        } else
        {
            Console.WriteLine("Couldn't load rooms from rooms file");
        }
    }

    internal static void SetRooms(GameState world, List<RoomEntry> rooms)
    {
        foreach (RoomEntry room in rooms) {
            if (room.X >= 0 && room.Y >= 0 && 
                room.X < world.RoomManager.Rooms.GetLength(0) && room.Y < world.RoomManager.Rooms.GetLength(1))
            {
                world.RoomManager.SetRoom(room.Room, room.X, room.Y);
            }
        }
    }


    private static void LoadNpcs(GameState world, string fileName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{fileName}");
        var npcs = JsonSerializer.Deserialize<Dictionary<string, NPC>>(json, options);
        if (npcs != null)
        {
            world.NPCManager.NPCs = npcs;
        }
    }


    private static void LoadQuests(GameState world, string fileName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{fileName}");
        var quests = JsonSerializer.Deserialize<Dictionary<string, Quest>>(json, options);
        if (quests != null)
        {
            world.QuestManager.Quests = quests;
        }
    }


    private static void LoadItems(GameState world, string fileName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{fileName}");
        var items = JsonSerializer.Deserialize<List<Item>>(json, options);
        
        if (items != null)
        {
            world.ItemManager.Items = items;
        }
    }
    // special function
    public static void LoadPlayer(GameState world, string directoryName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{Path.Combine(directoryName, "player.json")}");
        var player = JsonSerializer.Deserialize<int[]>(json, options);

        if (player != null)
        {
            world.Player = new Player(player[0], player[1], world);
        }

    }
}
