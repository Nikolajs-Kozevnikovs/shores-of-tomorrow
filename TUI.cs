using System.ComponentModel.DataAnnotations;
using System.IO;
using Spectre.Console;

namespace WorldOfZuul;

public static class TUI
{
  // required terminal dimensions
  private const int RequiredWidth = 132;
  private const int RequiredHeight = 43;

  // test function that emulates how it all works
  public static void Run()
  {
    // load all images required for render
    string bgPath = "./assets/graphics/img1.txt";   // background
    string mapPath = "./assets/graphics/map1.txt";  // minimap

    if (!File.Exists(bgPath))
      throw new FileNotFoundException($"background file not found: {bgPath}");
    if (!File.Exists(mapPath))
      throw new FileNotFoundException($"map overlay file not found: {mapPath}");

    List<List<Color>> bgColors = ParseTextImage(bgPath);
    List<List<Color>> mapColors = ParseTextImage(mapPath);

    // create a new canvas of the size 132x43
    var canvas = new Canvas(RequiredWidth, RequiredHeight);

    // get the background on canvas
    DrawAnsiLinesOntoCanvas(canvas, bgColors, 0, 0);

    // choose where to put the map
    int offsetX = RequiredWidth - mapColors[0].Count; // top‑right alignment
    int offsetY = 0;                           // top‑aligned

    // get the map on canvas
    DrawAnsiLinesOntoCanvas(canvas, mapColors, offsetX, offsetY);

    // render the canvas
    AnsiConsole.Write(canvas);

    // temp
    // draw subtitles below the canvas TEMPORARY SOLUTION
    string subtitle = "you stand before the ancient gate. what will you do?";
    int start = Math.Max(0, (RequiredWidth - subtitle.Length) / 2);
    string padded = new string(' ', start) + $"[bold]{subtitle}[/]";
    AnsiConsole.MarkupLine(padded);
  }

  // draw the canvas with specified colors
  private static void DrawAnsiLinesOntoCanvas(Canvas canvas, List<List<Color>> colorsList, int offsetX, int offsetY)
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
        line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
          .Select(item => item.Split(';', StringSplitOptions.RemoveEmptyEntries))
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
