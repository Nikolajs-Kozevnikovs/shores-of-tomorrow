namespace WorldOfZuul.Presentation;

using WorldOfZuul.Logic;
using System.Text.RegularExpressions;


public class LoadSave
{
    public static void SaveProgress(TUI tui, GameState World)
    {
        tui.WriteLine("Choose a name for your save: \n(English alphabet letters, numbers, underscores and dashes allowed)");
        Console.Write("> ");
        string? save_name = Console.ReadLine();
        while (save_name == null || save_name == "" || !IsValidSaveName(save_name))
        {
            tui.WriteLine("Invalid name!");
            Console.Write("> ");
            save_name = Console.ReadLine();
        }
        World.Save(save_name, tui);
        tui.WriteLine();
    }
    private static bool IsValidSaveName(string input)
    {
        return Regex.IsMatch(input, @"^[A-Za-z0-9_-]+$");
    }
        
    public static void ChooseSave(TUI tui, GameState World)
    {
        string[] existing_saves = World.GetSaves();
        tui.WriteLine("Choose a save:");
        foreach (string save in existing_saves)
        {
            tui.WriteLine(save);
        }

        Console.Write("> ");
        string? save_name = Console.ReadLine();
        while (save_name == null || !existing_saves.Contains(save_name))
        {
            tui.WriteLine("Can't find a save with the specified save name!");
            Console.Write("> ");
            save_name = Console.ReadLine();
        }

        World.LoadData(save_name);
    }
}