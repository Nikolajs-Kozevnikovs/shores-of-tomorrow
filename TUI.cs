using System.ComponentModel.DataAnnotations;
using System.IO;
using Spectre.Console;

namespace WorldOfZuul;

public class TUI
{
  // required terminal dimensions
  private const int RequiredWidth = 132;
  private const int RequiredHeight = 43;
  private readonly Canvas canvas = new(RequiredWidth, RequiredHeight);
  private List<List<Color>> Minimap { get; }

  public TUI()
  {
    // load the minimap
    string mapPath = "./assets/graphics/minimap.csv";
    if (!File.Exists(mapPath))
      throw new FileNotFoundException($"map overlay file not found: {mapPath}");

    Minimap = ParseTextImage(mapPath);

    LoadStartScreen();
    DrawCanvas();
  }


  public void DrawCanvas()
  {
    Console.Clear();
    AnsiConsole.Write(canvas);
  }


    // char[,] renderWindow = { {'w', 'a', 'f' },
    //                           {'r', 'b', 'd'} };
    // renderWindow[2, 3] = 'n';

  private void UpdateMinimap(Room currentRoom)
  {
    // choose where to put the map
    int offsetX = RequiredWidth - Minimap[0].Count; // top‑right alignment
    int offsetY = 0;                           // top‑aligned

    // get the map on canvas
    DrawAnsiLinesOntoCanvas(Minimap, offsetX, offsetY);

    // here we will need to use the position of current room to display where the main character is. Also display the tiles that are active for quests
    // Something like this:
    // canvas.setPixel(offsetX+currentPositionX, offsetY+currentPositionY, new Color(255, 0, 0))
  }


  public void UpdateBackground(Room currentRoom)
  {
    string bgPath = "./assets/graphics/" + currentRoom.Background;

    if (!File.Exists(bgPath))
      throw new FileNotFoundException($"background file not found: {bgPath}");

    List<List<Color>> bgColors = ParseTextImage(bgPath);

    DrawAnsiLinesOntoCanvas(bgColors, 0, 0);

    UpdateMinimap(currentRoom);
  }

  // maybe we won't need it after we get Room update and will have the currentRoom from the beginning
  public void LoadStartScreen()
  {
    string bgPath = "./assets/graphics/startScreen.csv";

    if (!File.Exists(bgPath))
      throw new FileNotFoundException($"background file not found: {bgPath}");

    List<List<Color>> bgColors = ParseTextImage(bgPath);

    DrawAnsiLinesOntoCanvas(bgColors, 0, 0);

    int offsetX = RequiredWidth - Minimap[0].Count; // top‑right alignment
    int offsetY = 0;
    DrawAnsiLinesOntoCanvas(Minimap, offsetX, offsetY);

  }


  // draw the canvas with specified colors
  private void DrawAnsiLinesOntoCanvas(List<List<Color>> colorsList, int offsetX, int offsetY)
  {
    for (int i = 0; i < colorsList.Count; i++)
    {
      for (int j = 0; j < colorsList[i].Count; j++)
      {
        canvas.SetPixel(j + offsetX, i + offsetY, colorsList[i][j]);
      }
    }
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
