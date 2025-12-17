namespace WorldOfZuul.Presentation
{
    using WorldOfZuul.Logic;
    using System.Text.RegularExpressions;

    public class Game
    {
        private readonly TUI tui = new();
        private readonly GameState World = new(7, 7);
        private readonly RandomEvents randomEvents = new();
        private bool isInDialogue = false;
        
        
        public void Play()
        {
            Parser parser = new();

            tui.WaitForCorrectTerminalSize();

            ChooseSave();

            tui.WriteLine(World.Player.CurrentRoom.Description ?? "None");
            PrintWelcome();

            tui.DrawCanvas();

            bool continuePlaying = true;
            while (continuePlaying)
            {
                continuePlaying = ProcessUserInput(parser);
                if (continuePlaying)
                {
                    randomEvents.TryTrigger(tui, World.Player.CurrentRoom, isInDialogue);
                }
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
                    tui.ClearDialogBoxLines();
                    tui.WriteLine(World.Player.CurrentRoom.Description ?? "Nothing to look at (devs forgot a description)");
                    break;

                case "back":
                    tui.ClearDialogBoxLines();
                    CatchMoveError(World.Player.Back());
                    break;
                case "north":
                case "south":
                case "east":
                case "west":
                    tui.ClearDialogBoxLines();
                    CatchMoveError(World.Player.Move(command.Name));
                    break;
                case "talk":
                    tui.ClearDialogBoxLines();
                    TalkToNPC();
                    break;

                case "help":
                    tui.ClearDialogBoxLines();
                    PrintHelp();
                    break;
                case "save":
                    tui.ClearDialogBoxLines();
                    SaveProgress();
                    break;
                case "quit":
                    tui.ClearDialogBoxLines();
                    tui.WriteLine("Thank you for playing World of Zuul!");
                    SaveProgress();
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
                isInDialogue = false;
                tui.WriteLine("No one is here!");
                return;
            }
            
            else if(World.Player.CurrentRoom.NPCs.Count > 0)
            {
                isInDialogue = true;
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

        private void SaveProgress()
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
        bool IsValidSaveName(string input)
        {
            return Regex.IsMatch(input, @"^[A-Za-z0-9_-]+$");
        }
        
        private void ChooseSave()
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




        private void PrintWelcome()
        {
            tui.WriteLine("Welcome to the World of Zuul!");
            tui.WriteLine("World of Zuul is a new, incredibly boring adventure game.");
            tui.WriteLine();

            PrintHelp();
        }

        private void PrintHelp()
        {
            tui.WriteLine("---------- Help Menu ---------");
            tui.WriteLine("Navigate by typing 'north', 'south', 'east', or 'west'.");
            tui.WriteLine("Type 'look' for more details.");
            tui.WriteLine("Type 'back' to go to the previous room.");
            tui.WriteLine("Type 'help' to print this message again.");
            tui.WriteLine("Type 'quit' to exit the game.");
            tui.WriteLine("------------------------------");
        }
    }
}
