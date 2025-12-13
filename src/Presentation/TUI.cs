namespace WorldOfZuul.Presentation;

using System.Text;
// using Spectre.Console;
using WorldOfZuul.Logic; // remove if collisions with local Color

public readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public Color(byte r, byte g, byte b) { R = r; G = g; B = b; }
}

public class TUI
{
    private const int RequiredWidth = 132;
    private const int RequiredHeight = 43;

    // two-dimensional array, stores Color values for each pixel on the screen.
    private readonly Color[,] pixelBuffer = new Color[RequiredWidth, RequiredHeight];

    private readonly string[] lines = new string[8];
    private int currentLineIndex = 0;

    private const int DialogWidth = 60; // fixed width for dialog box
    private const int DialogHeight = 7; // fixed height for dialog box
    private const int DialogPadding = 1;

    public TUI()
    {
        Clear();
        LoadStartScreen();
        DrawCanvas();
    }

    public void DrawCanvas()
    {
        Console.Clear();
        RenderBufferToConsole();
        PrintDialogBox(lines);
    }

    public void WaitForCorrectTerminalSize()
    {
        while (true)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            if (width < RequiredWidth || height < RequiredHeight)
            {
                Console.Clear();
                Console.WriteLine($"Terminal too small. Need at least {RequiredWidth}x{RequiredHeight}.");
            }
            else
            {
                Console.Clear();
                break;
            }

            Thread.Sleep(1000);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < lines.Length; i++) lines[i] = "";
        currentLineIndex = 0;
        for (int y = 0; y < RequiredHeight; y++)
            for (int x = 0; x < RequiredWidth; x++)
                pixelBuffer[x, y] = new Color(0, 0, 0);
    }

    public void WriteLine(string line = "\n")
    {
        // Clear all lines in the dialog box before outputting new content
        for (int i = 0; i < lines.Length; i++) lines[i] = "";
        currentLineIndex = 0;

        string[] newLines = line.Split('\n');
        foreach (string newLine in newLines)
        {
            if (currentLineIndex >= lines.Length)
            {
                for (int i = 1; i < lines.Length; i++) lines[i - 1] = lines[i];
                currentLineIndex--;
            }
            lines[currentLineIndex++] = newLine;
        }
        DrawCanvas();
    }

    // Wrap helper
    private static IEnumerable<string> WrapLine(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text)) { yield return ""; yield break; }
        int pos = 0;
        while (pos < text.Length)
        {
            int len = Math.Min(maxWidth, text.Length - pos);
            if (len == maxWidth)
            {
                int lastSpace = text.LastIndexOf(' ', pos + len - 1, len);
                if (lastSpace > pos)
                {
                    len = lastSpace - pos;
                }
            }
            yield return text.Substring(pos, len).Trim();
            pos += len;
            while (pos < text.Length && text[pos] == ' ') pos++;
        }
    }

    private void PrintDialogBox(string[] inputLines)
    {
        var wrapped = new List<string>();
        foreach (var l in inputLines)
        {
            foreach (var w in WrapLine(l ?? "", DialogWidth - DialogPadding * 2))
                wrapped.Add(w);
        }

        // clear dialog box area
        int dialogBoxRow = RequiredHeight - 2 - DialogHeight; // 2 rows above bottom of canvas
        int dialogBoxColumn = (RequiredWidth - DialogWidth) / 2;

        for (int y = 0; y < DialogHeight; y++)
        {
            string emptyLine = new string(' ', DialogWidth);
            Console.Write($"\x1B[{dialogBoxRow + y + 1};{dialogBoxColumn + 1}H{emptyLine}");
        }

        // print content lines
        for (int i = 0; i < DialogHeight - DialogPadding * 2; i++)
        {
            string content = i < wrapped.Count ? wrapped[i] : "";
            string sidePadding = new string(' ', DialogPadding);
            string line = sidePadding + content.PadRight(DialogWidth - DialogPadding * 2) + sidePadding;
            Console.Write($"\x1B[{dialogBoxRow + DialogPadding + i + 1};{dialogBoxColumn + 1}H{line}");
        }

        // put input prompt fully below the canvas
        int inputRow = RequiredHeight + 1;
        if (inputRow > Console.WindowHeight) inputRow = Console.WindowHeight;
        if (inputRow < 1) inputRow = 1;
        Console.Write($"\x1B[{inputRow};0H");
    }

    public void UpdateBackground(Room currentRoom)
    {
        string bgPath = "./assets/graphics/";
        bgPath += string.IsNullOrEmpty(currentRoom?.Background) ? "startScreen.csv" : currentRoom.Background;

        if (!File.Exists(bgPath))
            throw new FileNotFoundException($"background file not found: {bgPath}");

        var bgColors = ParseTextImage(bgPath);
        DrawColorsToBuffer(bgColors, 0, 0);
        DrawCanvas();
    }

    public void LoadStartScreen()
    {
        string bgPath = "./assets/graphics/startScreen.csv";
        if (!File.Exists(bgPath))
            throw new FileNotFoundException($"background file not found: {bgPath}");

        var bgColors = ParseTextImage(bgPath);
        DrawColorsToBuffer(bgColors, 0, 0);
    }

    private void DrawColorsToBuffer(List<List<Color>> colorsList, int offsetX, int offsetY)
    {
        for (int y = 0; y < colorsList.Count && y + offsetY < RequiredHeight; y++)
        {
            var row = colorsList[y];
            for (int x = 0; x < row.Count && x + offsetX < RequiredWidth; x++)
            {
                pixelBuffer[x + offsetX, y + offsetY] = row[x];
            }
        }
    }

    // Render pixelBuffer to console using background-colored spaces for squarish pixels
    private void RenderBufferToConsole()
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

    private static List<List<Color>> ParseTextImage(string fpath)
    {
        var result = File.ReadAllLines(fpath)
          .Select(line => line.Trim())
          .Where(line => !string.IsNullOrWhiteSpace(line))
          .Select(line =>
            line.Split(';', StringSplitOptions.RemoveEmptyEntries)
              .Select(item => item.Split(' ', StringSplitOptions.RemoveEmptyEntries))
              .Where(parts => parts.Length >= 3)
              .Select(parts =>
              {
                  bool rOk = byte.TryParse(parts[0], out var r);
                  bool gOk = byte.TryParse(parts[1], out var g);
                  bool bOk = byte.TryParse(parts[2], out var b);

                  if (!rOk || !gOk || !bOk)
                  {
                      Console.WriteLine($"Invalid color entry: [{string.Join(';', parts)}]");
                      return new Color(0, 0, 0);
                  }

                  return new Color(r, g, b);
              })
              .ToList()
        )
        .ToList();
        return result;
    }
}

