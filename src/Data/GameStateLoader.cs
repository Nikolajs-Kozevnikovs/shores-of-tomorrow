namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;

public static class GameStateLoader
{
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
    }

    // load rooms
    private class RoomsFile
    {
        public int width { get; set; }
        public int height { get; set; }
        public List<RoomEntry> ?rooms { get; set; }
    }

    private static void LoadRooms(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<RoomsFile>(json, options);

        if (data != null && data.rooms != null)
        {
            world.RoomManager.SetRooms(data.rooms);
        } else
        {
            Console.WriteLine("Couldn't load rooms from rooms file");
        }
    }

    // load npcs
    private class NpcFile
    {
        public Dictionary<string, NPC> npcs { get; set; } = [];
    }

    private static void LoadNpcs(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<NpcFile>(json, options);
        if (data != null)
        {
            world.NPCManager.Npcs = data.npcs;
        }
    }

    // load quests
    private class QuestFile
    {
        public Dictionary<string, Quest> quests { get; set; } = [];
    }

    private static void LoadQuests(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<QuestFile>(json, options);
        if (data != null)
        {
            world.QuestManager.Quests = data.quests;
        }
    }

    // load items
    private class ItemFile
    {
        public List<Item> items { get; set; } = [];
    }

    private static void LoadItems(GameState world, string fileName)
    {
        string json = File.ReadAllText($"./assets/data/{fileName}");
        var data = JsonSerializer.Deserialize<ItemFile>(json, options);
        
        if (data != null)
        {
            world.ItemManager.Items = data.items;
        }
    }
}
