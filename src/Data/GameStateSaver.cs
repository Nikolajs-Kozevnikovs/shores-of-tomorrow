// not tested yet!
namespace WorldOfZuul.Data;

using System.Text.Json;
using WorldOfZuul.Logic;

public static class GameStateSaver
{
    public static void Save(GameState world, string directory)
    {
        SaveRooms(world, Path.Combine(directory, "rooms.json"));
        SaveNpcs(world, Path.Combine(directory, "npcs.json"));
        SaveQuests(world, Path.Combine(directory, "quests.json"));
        SaveItems(world, Path.Combine(directory, "items.json"));
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

        string jsonString = JsonSerializer.Serialize(roomEntries);
        File.WriteAllText($"./assets/data/{fileName}", jsonString);
    }

    public static void SaveNpcs(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.NPCManager.NPCs);
        File.WriteAllText($"./assets/data/{fileName}", jsonString);
    }

    public static void SaveQuests(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.QuestManager.Quests);
        File.WriteAllText($"./assets/data/{fileName}", jsonString);
    }

    public static void SaveItems(GameState world, string fileName)
    {
        string jsonString = JsonSerializer.Serialize(world.ItemManager.Items);
        File.WriteAllText($"./assets/data/{fileName}", jsonString);
    }
     
}