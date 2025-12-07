namespace WorldOfZuul
{
    using WorldOfZuul.Presentation;
    using WorldOfZuul.Logic;

    public class Game
    {
        private readonly TUI tui = new();
        private readonly GameState World = new(7, 7);
        
        
        public void Play()
        {
            Parser parser = new();   

            PrintWelcome();
            tui.WaitForCorrectTerminalSize();
            
            tui.WriteLine(World.Player.CurrentRoom.Description ?? "None");
            tui.DrawCanvas();

            bool continuePlaying = true;
            while (continuePlaying)
            {
                continuePlaying = ProcessUserInput(parser);
                tui.DrawCanvas();
            }
        }

        private bool ProcessUserInput(Parser parser)
        {
            Console.Write("> ");

            string? input = Console.ReadLine();
            // check for empty input
            if (string.IsNullOrEmpty(input))
            {
                tui.WriteLine("Please enter a command.");
                return true;
            }
            // parse into command
            Command? command = parser.GetCommand(input);
            // check if command is invalid
            if (command == null || command.Name == null)
            {
                tui.WriteLine("I don't know that command.");
                return true;
            }
            // execute the command
            return HandleCommand(command);
        }

        private bool HandleCommand(Command command)
        {
            switch (command.Name)
            {
                case "look":
                    tui.WriteLine(World.Player.CurrentRoom.Description ?? "Nothing to look at (devs forgot a description)");
                    break;

                case "back": 
                    CatchMoveError(World.Player.Back());
                    break;
                case "north":
                case "south":
                case "east":
                case "west":
                    CatchMoveError(World.Player.Move(command.Name));
                    break;
                case "talk":
                    TalkToNPC();
                    break;

                case "help":
                    PrintHelp();
                    break;
                
                case "quit":
                    tui.WriteLine("Thank you for playing World of Zuul!");
                    return false;

                default:
                    tui.WriteLine("I don't know what command.");
                    break;
            }

            return true;
        }

        // TBD
        private void TalkToNPC()
        {
            if (World.Player.CurrentRoom.NPCs.Count == 0)
            {
                tui.WriteLine("No one is here!");
                return;
            } 
                    
            tui.WriteLine("Dialogues are not implemented yet for multiple NPCs in one room");

            // // fix for multiple NPCs
            // if (r.RoomNPC.quest != null && r.RoomNPC.quest.State == QuestState.Pending)
            // {
            //     r.RoomNPC.Talk(tui);
            // }
            // else
            // {
            //     tui.WriteLine("no quest sorry");
            //     //World.Player.CurrentRoom.RoomNPC.Dialogue1();
            // }
        }
        
        private void CatchMoveError(string? errorText)
        {
            if (errorText == null)
            {
               
                tui.WriteLine(World.Player.CurrentRoom.Description ?? "None");
                tui.UpdateBackground(World.Player.CurrentRoom);
                return;
            }

            tui.WriteLine(errorText);
        }

        private void PrintWelcome()
        {
            tui.WriteLine("Welcome to the World of Zuul!");
            tui.WriteLine("World of Zuul is a new, incredibly boring adventure game.");
            PrintHelp();
            tui.WriteLine();
        }

        private void PrintHelp()
        {
            tui.WriteLine("Navigate by typing 'north', 'south', 'east', or 'west'.");
            tui.WriteLine("Type 'look' for more details.");
            tui.WriteLine("Type 'back' to go to the previous room.");
            tui.WriteLine("Type 'help' to print this message again.");
            tui.WriteLine("Type 'quit' to exit the game.");
        }
    }
}
