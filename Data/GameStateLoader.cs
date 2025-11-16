using System.Text.Json;
using System.Text.Json.Serialization;

public static class GameStateLoader
{
    private static JsonSerializerOptions options = new JsonSerializerOptions
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
    }

    // ------------------ ROOM LOADING ------------------
    private class RoomsFile
    {
        public int width { get; set; }
        public int height { get; set; }
        public List<RoomEntry> rooms { get; set; }
    }

    private class RoomEntry : Room
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    private static void LoadRooms(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<RoomsFile>(json, options);

        world.RoomManager = new RoomManager(data.width, data.height, world);

        foreach (var r in data.rooms)
        {
            world.RoomManager.Rooms[r.x, r.y] = r;
        }
    }

    // ------------------ NPC LOADING ------------------
    private class NpcFile
    {
        public Dictionary<string, NPC> npcs { get; set; }
    }

    private static void LoadNpcs(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<NpcFile>(json, options);

        world.NPCManager.Npcs = data.npcs;
    }

    // ------------------ QUEST LOADING ------------------
    private class QuestFile
    {
        public Dictionary<string, Quest> quests { get; set; }
    }

    private static void LoadQuests(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<QuestFile>(json, options);

        world.QuestManager.Quests = data.quests;
    }

    // ------------------ ITEMS LOADING ------------------
    private class ItemFile
    {
        public List<Item> items { get; set; }
    }

    private static void LoadItems(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<ItemFile>(json, options);

        world.ItemManager.Items = data.items;
    }
}
