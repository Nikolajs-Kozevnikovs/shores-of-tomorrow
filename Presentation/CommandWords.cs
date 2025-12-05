namespace WorldOfZuul
{
    public class CommandWords
    {
        public List<string> ValidCommands { get; } = new List<string> { "north", "east", "south", "west", "look", "back", "quit", "help", "talk" };

        public bool IsValidCommand(string command)
        {
            return ValidCommands.Contains(command);
        }
    }
}
// how dores this work?