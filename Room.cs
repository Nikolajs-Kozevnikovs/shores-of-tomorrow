namespace WorldOfZuul;

public class Room
{
    public string TileIdentifier { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string? Background { get; private set; }
    public NPC? RoomNPC { get; private set; }

    public Room(string tileIdentifier, string shortDesc, string longDesc, string? background = null)
    {
        TileIdentifier = tileIdentifier;
        ShortDescription = shortDesc;
        LongDescription = longDesc;
        Background = background;
    }

    // Predefined room types
    public static readonly Room Ocean = new("O", "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv");
    public static readonly Room CoralReef = new("C", "Coral Reef", "You find yourself amidst a vibrant coral reef, teeming with colorful fish and marine life.", "roomBackground2.csv");
    public static readonly Room SeaShore = new("S", "Sea Shore", "You stand on a sandy sea shore, with gentle waves lapping at your feet and the salty breeze in the air.", "roomBackground3.csv");
    public static readonly Room House = new("H", "House", "You are inside a cozy house, bla bla bla", "roomBackground4.csv");
    public static readonly Room Boat = new("B", "Boat", "You are on a small boat, gently rocking on the waves.", "roomBackground5.csv");
    public static readonly Room Lake = new("L", "Lake", "You are at a serene lake, surrounded by trees and mountains.");
    public static readonly Room MarineLaboratory = new("M", "Marine Laboratory", "You are in a high-tech marine laboratory, filled with equipment and research materials.");
    public static readonly Room RecyclingCentre = new("R", "Recycling Centre", "You are at a recycling centre, where waste is processed and sorted.");
    public static readonly Room Townhall = new("T", "Townhall", "You are in the townhall, a place for community meetings and events.");
    public static readonly Room Lake2 = new("W", "Lake 2", "You are at another peaceful lake, with clear water and abundant wildlife.");
    public static readonly Room OldMansHouse = new("X", "Old Man's House", "An old man's small cottage.");
    public static readonly Room Factory = new("F", "Factory", "You are in a bustling factory, with machines whirring and workers busy at their tasks.");
    public static readonly Room Blank = new("-", "Blank", "You are in an empty space, nothing to see here.");

    // Helper to create a fresh Room instance from a prototype so each map tile
    // gets its own mutable Room (so NPCs, state etc. aren't shared).
    private static Room Clone(Room RoomToBeCloned)
    {
        return new Room(RoomToBeCloned.TileIdentifier, RoomToBeCloned.ShortDescription, RoomToBeCloned.LongDescription, RoomToBeCloned.Background);
    }

    // The world map: rows (Y) x columns (X)
    // Map[y, x]
    // Different Room instances per cell (clones of templates)
    public static readonly Room?[,] Map = new Room?[,]
    {
        { Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(CoralReef) },
        { Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean) },
        { Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean) },
        { Clone(Ocean), Clone(Ocean), Clone(Boat),  Clone(Ocean), Clone(Ocean), Clone(Ocean), Clone(Ocean) },
        { Clone(SeaShore), Clone(SeaShore), Clone(SeaShore), Clone(SeaShore), Clone(SeaShore), Clone(SeaShore), Clone(SeaShore) },
        { Clone(House), Clone(Blank), Clone(Blank), Clone(Blank), Clone(Blank), Clone(Blank), Clone(RecyclingCentre) },
        { Clone(Lake), Clone(MarineLaboratory), Clone(Blank), Clone(Townhall), Clone(Blank), Clone(Blank), Clone(Blank) },
        { Clone(Factory), Clone(Blank), Clone(Blank), Clone(Blank), Clone(Blank), Clone(OldMansHouse), Clone(Lake2) },
    };

    public static int Rows => Map.GetLength(0);
    public static int Columns => Map.GetLength(1);

    public static Room? GetRoomAt(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Columns) return null;
        return Map[row, col];
    }

    public static string GetShortDescription(int row, int col)
    {
        var r = GetRoomAt(row, col);
        return r?.ShortDescription ?? "None";
    }

    public static string GetLongDescription(int row, int col)
    {
        var r = GetRoomAt(row, col);
        return r?.LongDescription ?? "None";
    }

    public void SetNPC(NPC npc)
    {
        RoomNPC = npc;
    }

    public void RemoveNPC()
    {
        RoomNPC = null;
    }
}

