namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;


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
        LoadItems();
        LoadNpcs();
        LoadRooms(world, Path.Combine(directory, "rooms.json"));
        LoadQuests(world, Path.Combine(directory, "quests.json"));
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
    foreach (RoomEntry roomEntry in rooms)
    {
        // Convert item ID strings into actual Item objects
        List<Item> items = roomEntry.Room.ItemIds
            .Select(id => ItemRegistry.CreateItem(id))
            .ToList();

        roomEntry.Room.Items = items;

        // Place the room in the world's 2D array
        if (roomEntry.X >= 0 && roomEntry.Y >= 0 &&
            roomEntry.X < world.RoomManager.Rooms.GetLength(0) &&
            roomEntry.Y < world.RoomManager.Rooms.GetLength(1))
        {
            world.RoomManager.SetRoom(roomEntry.Room, roomEntry.X, roomEntry.Y);
        }
    }
}


    private static void LoadNpcs()
    {
        string json = File.ReadAllText($"./assets/npcs.json");
        var npcs = JsonSerializer.Deserialize<List<NPC>>(json, options);
        if (npcs != null)
        {
            foreach (NPC npc in npcs)
            {  
                NPCRegistry.Register(npc);
            }
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


    private static void LoadItems()
    {
        string json = File.ReadAllText($"./assets/items.json");
        var items = JsonSerializer.Deserialize<List<Item>>(json, options);
        
        if (items != null)
        {
            foreach (Item item in items)
            {
                ItemRegistry.Register(item.Id, item.Name);
            }
        }
    }
    
    

    public static void LoadPlayer(GameState world, string directoryName)
    {
        string json = File.ReadAllText($"{SAVE_PATH}{Path.Combine(directoryName, "player.json")}");
        var player = JsonSerializer.Deserialize<PlayerEntry>(json, options);

        if (player != null)
        {
            world.Player = new Player(
                x: player.X,
                y: player.Y,
                activeQuestName: player.ActiveQuestName,
                inventory: player.ItemIds.Select(itemId => ItemRegistry.CreateItem(itemId)).ToList() ?? [],
                world: world
            );
        }

    }
}
