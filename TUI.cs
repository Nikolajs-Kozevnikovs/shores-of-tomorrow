using System.Runtime.CompilerServices;

namespace WorldOfZuul;

class TUI
{
  public static string[] ReadImage(string fname)
  {
    string path = "./assets/graphics/";
    string[] content = File.ReadAllLines(fname + path);
    return content;
  }

  public static void RenderScreen()
  {
    // tbd
    string[] canvas = ReadImage("background.txt");

    
    // char[,] renderWindow = { {'w', 'a', 'f' },
    //                           {'r', 'b', 'd'} };
    // renderWindow[2, 3] = 'n';

    // Render(renderWindow);
  }
}

