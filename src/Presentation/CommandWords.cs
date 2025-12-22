namespace WorldOfZuul.Presentation
{
    internal class CommandWords
    {
        static List<string> ValidCommands = new List<string> { "north", "east", "south", "west", "look", "back", "quit", "help", "talk", "save", "take", "inventory"};
        
        public static bool IsValidCommand(string command)
        {
            return ValidCommands.Contains(command);
        }
    }
}