namespace WorldOfZuul.Presentation;

using System.Text;
using System.Collections.Generic;
using WorldOfZuul.Logic;

public readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public Color(byte r, byte g, byte b) { R = r; G = g; B = b; }
}

public static class TUI
{
    // layout constants
    private const int RequiredWidth = 132;
    private const int RequiredHeight = 43;
    private const int DialogWidth = 70;
    private const int DialogHeight = 12;
    private const int DialogPadding = 1;

    // pixel buffer
    private static readonly Color[,] pixelBuffer = new Color[RequiredWidth, RequiredHeight];

    // colour map for minimap tiles
    private static readonly Dictionary<char, Color> MiniMapTileColors = new()
    {
        { 'B', new Color(160,  60,  60) }, // boat
        { 'C', new Color(230,  50, 160) }, // coralReef
        { 'D', new Color(170, 140, 160) }, // road
        { 'F', new Color( 80,  90,  80) }, // factory
        { 'H', new Color(140, 120, 140) }, // highway
        { 'L', new Color(220, 230, 250) }, // laboratory
        { 'N', new Color(120,  90, 100) }, // house1
        { 'O', new Color( 30, 120, 190) }, // ocean
        { 'P', new Color(210,  30,  40) }, // foodCharity
        { 'Q', new Color( 50,  20, 170) }, // ocean2
        { 'R', new Color( 40, 140, 110) }, // recyclingCentre
        { 'S', new Color(250, 200, 130) }, // seashore
        { 'T', new Color(220, 100,  90) }, // townhall
        { 'X', new Color(150, 200, 240) }, // lake1
        { 'Y', new Color(140, 210, 230) }, // lake2
        { '-', new Color(  0,   0,   0) }   // void
    };

    // reference to the current game state
    public static GameState? CurrentWorld { get; set; }

    // dialog handling
    private static readonly List<string> lines = new List<string>();

    public static void StartGame()
    {
        Clear();
        LoadStartScreen();
        DrawCanvas();
    }

    public static void DrawCanvas()
    {
        Console.Clear();
        RenderBufferToConsole();
        PrintDialogBox(lines);
    }

    public static void WaitForCorrectTerminalSize()
    {
        while (true)
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;
            if (w < RequiredWidth || h < RequiredHeight)
            {
                Console.Clear();
                Console.WriteLine($"terminal too small – need {RequiredWidth}x{RequiredHeight}");
            }
            else { Console.Clear(); break; }
            Thread.Sleep(1000);
        }
    }

    public static void Clear()
    {
        lines.Clear();
        for (int y = 0; y < RequiredHeight; y++)
            for (int x = 0; x < RequiredWidth; x++)
                pixelBuffer[x, y] = new Color(0, 0, 0);
    }

    public static void WriteLine(string line = "\n")
    {
        foreach (var part in line.Split('\n'))
        {
            var wrapped = WrapLine(part, DialogWidth - DialogPadding * 2);
            lines.AddRange(wrapped);
            while (lines.Count > DialogHeight - DialogPadding * 2)
                lines.RemoveAt(0);
        }
        DrawCanvas();
    }

    public static void ClearDialogBoxLines()
    {
        lines.Clear();
        DrawCanvas();
    }

    public static void UpdateBackground(Room currentRoom)
    {
        if (currentRoom == null) { Console.WriteLine("null room"); return; }

        string path = "./assets/graphics/" +
                      (string.IsNullOrEmpty(currentRoom.Background)
                       ? "startScreen.csv"
                       : currentRoom.Background);

        if (!File.Exists(path))
        {
            Console.WriteLine($"missing bg {path}");
            path = "./assets/graphics/startScreen.csv";
        }

        var bg = ParseTextImage(path);
        DrawColorsToBuffer(bg, 0, 0);
        DrawMiniMapOverlay(); // draw map after background
        DrawCanvas();
    }

    public static void LoadStartScreen()
    {
        string path = "./assets/graphics/startScreen.csv";
        if (!File.Exists(path)) throw new FileNotFoundException(path);
        var bg = ParseTextImage(path);
        DrawColorsToBuffer(bg, 0, 0);
    }

    // background helpers
    private static void DrawColorsToBuffer(List<List<Color>> src, int ox, int oy)
    {
        for (int y = 0; y < src.Count && y + oy < RequiredHeight; y++)
            for (int x = 0; x < src[y].Count && x + ox < RequiredWidth; x++)
                pixelBuffer[x + ox, y + oy] = src[y][x];
    }

    private static void RenderBufferToConsole()
    {
        var sb = new StringBuilder(RequiredWidth * RequiredHeight * 8);
        for (int y = 0; y < RequiredHeight; y++)
        {
            for (int x = 0; x < RequiredWidth; x++)
            {
                var c = pixelBuffer[x, y];
                sb.Append($"\x1B[48;2;{c.R};{c.G};{c.B}m ");
            }
            sb.Append("\x1B[0m");
            if (y < RequiredHeight - 1) sb.AppendLine();
        }
        Console.SetCursorPosition(0, 0);
        Console.Write(sb.ToString());
    }

    private static List<List<Color>> ParseTextImage(string file)
    {
        return File.ReadAllLines(file)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split(';', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                         .Where(p => p.Length >= 3)
                         .Select(p =>
                         {
                             byte.TryParse(p[0], out var r);
                             byte.TryParse(p[1], out var g);
                             byte.TryParse(p[2], out var b);
                             return new Color(r, g, b);
                         }).ToList())
            .ToList();
    }

    // dialog helpers
    private static IEnumerable<string> WrapLine(string txt, int max)
    {
        if (string.IsNullOrEmpty(txt)) yield return "";
        int pos = 0;
        while (pos < txt.Length)
        {
            int len = Math.Min(max, txt.Length - pos);
            if (len == max)
            {
                int sp = txt.LastIndexOf(' ', pos + len - 1, len);
                if (sp > pos) len = sp - pos;
            }
            yield return txt.Substring(pos, len).Trim();
            pos += len;
            while (pos < txt.Length && txt[pos] == ' ') pos++;
        }
    }

    private static void PrintDialogBox(IEnumerable<string> src)
    {
        var wrapped = new List<string>();
        foreach (var l in src)
            foreach (var w in WrapLine(l ?? "", DialogWidth - DialogPadding * 2))
                wrapped.Add(w);

        int top = RequiredHeight - 2 - DialogHeight;
        int left = (RequiredWidth - DialogWidth) / 2;

        for (int y = 0; y < DialogHeight; y++)
        {
            string empty = new string(' ', DialogWidth);
            Console.Write($"\x1B[{top + y + 1};{left + 1}H{empty}");
        }

        for (int i = 0; i < DialogHeight - DialogPadding * 2; i++)
        {
            string txt = i < wrapped.Count ? wrapped[i] : "";
            string pad = new string(' ', DialogPadding);
            string line = pad + txt.PadRight(DialogWidth - DialogPadding * 2) + pad;
            Console.Write($"\x1B[{top + DialogPadding + i + 1};{left + 1}H{line}");
        }

        int inputRow = RequiredHeight + 1;
        if (inputRow > Console.WindowHeight) inputRow = Console.WindowHeight;
        if (inputRow < 1) inputRow = 1;
        Console.Write($"\x1B[{inputRow};0H");
    }


    // build colour matrix for the whole world
    private static Color[,] BuildMiniMapBuffer(GameState world)
    {
        int width  = world.RoomManager.Rooms.GetLength(0); // east‑west
        int height = world.RoomManager.Rooms.GetLength(1); // north‑south

        // buffer indexed as [horizontal, vertical]
        var buf = new Color[width, height];

        for (int x = 0; x < width; x++) // horizontal (east‑west)
        {
            for (int y = 0; y < height; y++) // vertical (north‑south)
            {
                var room = world.RoomManager.GetRoom(x, y);
                if (room == null || room.TileIdentifier == '-')
                {
                    buf[x, y] = new Color(0, 0, 0);
                    continue;
                }

                // base colour for the tile type
                if (!MiniMapTileColors.TryGetValue(room.TileIdentifier, out var col))
                    col = new Color(100, 100, 100);

                // highlight player position
                if (world.Player.X == x && world.Player.Y == y)
                    col = new Color(255, 255, 0); // bright yellow

                buf[x, y] = col;
            }
        }

        return buf;
    }

    // copy minimap onto pixel buffer, 2x1 cells, margin 1 top, 2 right
    private static void DrawMiniMapOverlay()
    {
        if (CurrentWorld == null) return;

        var mini = BuildMiniMapBuffer(CurrentWorld);
        int cellW = 2; // each map cell occupies two horizontal characters
        int mapW = mini.GetLength(0) * cellW;
        int mapH = mini.GetLength(1);

        // 2 columns from the right edge, 1 row from the top edge
        int startX = RequiredWidth - mapW - 2;
        int startY = 1;

        for (int cx = 0; cx < mini.GetLength(0); cx++) // columns (west‑east)
            for (int cy = 0; cy < mini.GetLength(1); cy++) // rows (north‑south)
            {
                int dstX = startX + cx * cellW;
                int dstY = startY + cy;

                if (dstX < 0 || dstY < 0 ||
                    dstX + 1 >= RequiredWidth || dstY >= RequiredHeight)
                    continue;

                pixelBuffer[dstX,     dstY] = mini[cx, cy];
                pixelBuffer[dstX + 1, dstY] = mini[cx, cy];
            }
    }
}
