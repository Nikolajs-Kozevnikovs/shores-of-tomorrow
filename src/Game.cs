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
            tui.DrawCanvas();
            
            Room currentRoom = World.RoomManager.GetCurrentRoom();
            if (currentRoom == null)
            {
                Console.WriteLine("Couldn't  fetch curernt room, breaking");
                return;
            }
            tui.WriteLine(currentRoom.Description ?? "None");

            bool continuePlaying = true;
            while (continuePlaying)
            {
                Command ?command = ProcessTurn(parser, ref currentRoom);

                if (command == null)
                {
                    continue;
                }

                // switch was here
                if (HandleCommand(command, ref currentRoom) == false)
                {
                    continuePlaying = false;
                }
                tui.DrawCanvas();
            }

            tui.WriteLine("Thank you for playing World of Zuul!");
        }

        private Command? ProcessTurn(Parser parser, ref Room currentRoom)
        {
            Console.Write("> ");

            string? input = Console.ReadLine();
            // check for empty input
            if (string.IsNullOrEmpty(input))
            {
                tui.WriteLine("Please enter a command.");
                tui.DrawCanvas();
                return null;
            }
            // parse into command
            Command? command = parser.GetCommand(input);
            // check if command is invalid
            if (command == null)
            {
                tui.WriteLine("I don't know that command.");
                tui.DrawCanvas();
                return null;
            }
            return command;
        }

        private bool HandleCommand(Command command, ref Room currentRoom)
        {
            switch (command.Name)
            {
                case "look":
                    tui.WriteLine(currentRoom.Description ?? "Nothing to look at");
                    tui.DrawCanvas(); // might be totally unnecessary
                    break;

                case "back": 
                    World.Player.Back(tui);
                    currentRoom = World.RoomManager.GetCurrentRoom();
                    tui.WriteLine(currentRoom.Description ?? "None");
                    tui.UpdateBackground(currentRoom);
                    break;

                case "north":
                case "south":
                case "east":
                case "west":
                    World.Player.Move(command.Name, tui); // doesn't change current room
                    currentRoom = World.RoomManager.GetCurrentRoom();
                    tui.WriteLine(currentRoom.Description ?? "None");
                    tui.UpdateBackground(currentRoom);
                    break;

                case "talk":
                    
                    if (currentRoom.NPCs.Count == 0)
                    {
                        tui.WriteLine("No one is here!");
                        break;
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
                    //     //currentRoom.RoomNPC.Dialogue1();
                    // }
                    break;

                case "help":
                    PrintHelp();
                    break;
                
                case "quit":
                    return false;

                default:
                    tui.WriteLine("I don't know what command.");
                    break;
            }

            return true;
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
