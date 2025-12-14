namespace WorldOfZuul.Presentation
{
    public class Parser
    {
        public Command? GetCommand(string inputLine)
        {
            string[] words = inputLine.Split();

            if (words.Length == 0 || !CommandWords.IsValidCommand(words[0]))
            {
                return null;
            }

            if (words.Length > 1)
            {
                return new Command(words[0], words[1]);
            }

            return new Command(words[0]);
        }
    }

}
