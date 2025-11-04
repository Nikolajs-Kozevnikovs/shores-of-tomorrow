namespace WorldOfZuul
{
    public class Room
    {
        public char TileIdentifier { get; private set; }
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string? Background { get; }
        public NPC? RoomNPC { get; private set ; }


        public Room(char tileIdentifier, string shortDesc, string longDesc, string background)
        {
            TileIdentifier = tileIdentifier;
            ShortDescription = shortDesc;
            LongDescription = longDesc;
            Background = background;
        }

        public Room(char tileIdentifier, string shortDesc, string longDesc)
        {
            TileIdentifier = tileIdentifier;
            ShortDescription = shortDesc;
            LongDescription = longDesc;
        }
        
        public static Room Ocean = new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.");
        public static Room CoralReef = new('C', "Coral Reef", "You find yourself amidst a vibrant coral reef, teeming with colorful fish and marine life.");
        public static Room SeaShore = new('S', "Sea Shore", "You stand on a sandy sea shore, with gentle waves lapping at your feet and the salty breeze in the air.");
        public static Room House = new('H', "House", "You are inside a cozy house, bla bla bla");
        public static Room Boat = new('B', "Boat", "You are on a small boat, gently rocking on the waves.");
        public static Room Lake = new('L', "Lake", "You are at a serene lake, surrounded by trees and mountains."); // CloseLake, illegal fishing
        public static Room MarineLaboratory = new('M', "Marine Laboratory", "You are in a high-tech marine laboratory, filled with equipment and research materials.");
        public static Room RecyclingCentre = new('R', "Recycling Centre", "You are at a recycling centre, where waste is processed and sorted.");
        public static Room Townhall = new('T', "Townhall", "You are in the townhall, a place for community meetings and events.");
        public static Room Lake2 = new('W', "Lake 2", "You are at another peaceful lake, with clear water and abundant wildlife."); // Further away, legal fishing
        public static Room OldMansHouse = new('X', "Old Man's House", "bla bla bla");
        public static Room Factory = new('F', "Factory", "You are in a bustling factory, with machines whirring and workers busy at their tasks.");
        public static Room Blank = new('-', "Blank", "You are in an empty space, nothing to see here.");

        private readonly char[][] Rooms = [
            /* ---TILE IDENTIFIERS:---
            OCEAN TILES = "O" // CORAL TILE = "C" // SEA SHORE TILE = "S"
            HOUSE TILE = "H" // BOAT TILE = "B" // LAKE TILE = "L"
            MARINE LABORATORY TILE = "M" // RECYCLING CENTRE TILE = "R" // TOWNHALL TILE = "T"
            LAKE 2 TILE = "W" // OLD MAN'S HOUSE TILE = "X" // FACTORY TILE = "F" // BLANK TILE = "-"
            */
            ['O','O','O','O','O','O','C'],
            ['O','O','O','O','O','O','O'],
            ['O','O','O','O','O','O','O'],
            ['O','O','O','B','O','O','O'],
            ['S','S','S','S','S','S','S'],
            ['H','-','-','-','-','-','R'],
            ['L','M','-','T','-','-','-'],
            ['F','-','-','-','-','X','W'],
        ];

        private string GetShortDescription(int x, int y)
        {
            if (y < 0 || y >= Rooms.Length || x < 0 || x >= Rooms[y].Length)
            {
                return "None";
            }

            char tile = Rooms[y][x]; // Get the tile identifier at the specified coordinates

            return tile switch
            {
                'O' => Ocean.ShortDescription,
                'C' => CoralReef.ShortDescription,
                'S' => SeaShore.ShortDescription,
                'H' => House.ShortDescription,
                'B' => Boat.ShortDescription,
                'L' => Lake.ShortDescription,
                'M' => MarineLaboratory.ShortDescription,
                'R' => RecyclingCentre.ShortDescription,
                'T' => Townhall.ShortDescription,
                'W' => Lake2.ShortDescription,
                'X' => OldMansHouse.ShortDescription,
                'F' => Factory.ShortDescription,
                '-' => Blank.ShortDescription,
                _ => "Unknown"
            };
        }

        private string GetLongDescription(int x, int y)
        {
            if (y < 0 || y >= Rooms.Length || x < 0 || x >= Rooms[y].Length)
            {
                return "None";
            }

            char tile = Rooms[y][x]; // Get the tile identifier at the specified coordinates

            return tile switch
            {
                'O' => Ocean.LongDescription,
                'C' => CoralReef.LongDescription,
                'S' => SeaShore.LongDescription,
                'H' => House.LongDescription,
                'B' => Boat.LongDescription,
                'L' => Lake.LongDescription,
                'M' => MarineLaboratory.LongDescription,
                'R' => RecyclingCentre.LongDescription,
                'T' => Townhall.LongDescription,
                'W' => Lake2.LongDescription,
                'X' => OldMansHouse.LongDescription,
                'F' => Factory.LongDescription,
                '-' => Blank.LongDescription,
                _ => "Unknown"
            };
        }

        public void GetExits(int X, int Y)
        {
            // Print short descriptions (names)
            Console.WriteLine(GetShortDescription(X, Y)); // Current Room
            Console.WriteLine(GetShortDescription(X, Y - 1)); // North Room
            Console.WriteLine(GetShortDescription(X + 1, Y)); // East Room
            Console.WriteLine(GetShortDescription(X, Y + 1)); // South Room
            Console.WriteLine(GetShortDescription(X - 1, Y)); // West Room
            Console.WriteLine(GetShortDescription(X + 1, Y - 1)); // North East Room
            Console.WriteLine(GetShortDescription(X + 1, Y + 1)); // South East Room
            Console.WriteLine(GetShortDescription(X - 1, Y + 1)); // South West Room
            Console.WriteLine(GetShortDescription(X - 1, Y - 1)); // North West Room
        }

        /*
        public char GetNorthRoomTile(int X, int Y)
        {
            return Rooms[Y - 1][X]; // Return North Room Tile
        }

        public char GetEastRoomTile(int X, int Y)
        {
            return Rooms[Y][X + 1]; // Return East Room Tile
        }

        public char GetSouthRoomTile(int X, int Y)
        {
            return Rooms[Y + 1][X]; // Return South Room Tile
        }

        public char GetWestRoomTile(int X, int Y)
        {
            return Rooms[Y][X - 1]; // Return West Room Tile
        }

        public char GetNorthEastRoomTile(int X, int Y)
        {
            return Rooms[Y - 1][X + 1]; // Return North East Room Tile
        }

        public char GetSouthEastRoomTile(int X, int Y)
        {
            return Rooms[Y + 1][X + 1]; // Return South East Room Tile
        }

        public char GetSouthWestRoomTile(int X, int Y)
        {
            return Rooms[Y + 1][X - 1]; // Return South West Room Tile
        }

        public char GetNorthWestRoomTile(int X, int Y)
        {
            return Rooms[Y - 1][X - 1]; // Return North West Room Tile
        }
        */

        /*
        public void SetExits(Room? north, Room? east, Room? south, Room? west)
        {
            SetExit("north", north);
            SetExit("east", east);
            SetExit("south", south);
            SetExit("west", west);
        }

        public void SetExit(string direction, Room? neighbor)
        {
            if (neighbor != null)
                Exits[direction] = neighbor;
        }

        public void SetNPC(NPC npc) {
            RoomNPC = npc;
        }

        public void RemoveNPC(NPC npc) {
            RoomNPC = null;
        }
    }
}
