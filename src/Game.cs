namespace WorldOfZuul
{
    using WorldOfZuul.Presentation;
    using WorldOfZuul.Logic;
    using WorldOfZuul.Data;

    public class Game
    {
        // currentRoom: [row, col]
        private int[]? previousCoords = null;
        private readonly TUI tui = new();
        private readonly GameState World = new(7, 7);

        public Game()
        {
            // load things from json
            GameStateLoader.Load(World, "/");
        }
        
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
                // tui.DrawCanvas();
                Console.Write("> ");

                string? input = Console.ReadLine();
                // check for empty input
                if (string.IsNullOrEmpty(input))
                {
                    tui.WriteLine("Please enter a command.");
                    tui.DrawCanvas();
                    continue;
                }
                // parse into command
                Command? command = parser.GetCommand(input);
                // check if command is invalid
                if (command == null)
                {
                    tui.WriteLine("I don't know that command.");
                    tui.DrawCanvas();
                    continue;
                }

                switch (command.Name)
                {
                    case "look":
                        tui.WriteLine(currentRoom.Description ?? "Nothing to look at");
                        tui.DrawCanvas();
                        break;

                    case "back": 
                        if (previousCoords == null)
                        {
                            tui.WriteLine("You can't go back from here!");
                        } else
                        {
                            World.PlayerX = previousCoords[0];
                            World.PlayerY = previousCoords[1];
                            previousCoords = null;
                            currentRoom = World.RoomManager.GetCurrentRoom();
                            tui.WriteLine(currentRoom.Description ?? "None");
                            tui.UpdateBackground(currentRoom);
                        }
                        break;

                    case "north":
                    case "south":
                    case "east":
                    case "west":
                        Move(command.Name); // doesn't change current room
                        currentRoom = World.RoomManager.GetCurrentRoom();
                        tui.WriteLine(currentRoom.Description ?? "None");
                        tui.UpdateBackground(currentRoom);
                        break;

                    case "talk":
                        
                        if (currentRoom.NPCs.Count == 0)
                        {
                            tui.WriteLine("No one is here!");
                            break;
                        } else
                        {
                            tui.WriteLine("Dialogues are not implemented yet for multiple NPCs in one room");
                        }
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

                    case "quit":
                        continuePlaying = false;
                        break;

                    case "help":
                        PrintHelp();
                        tui.DrawCanvas();
                        break;

                    default:
                        tui.WriteLine("I don't know what command.");
                        break;
                }
                tui.DrawCanvas();
            }

            tui.WriteLine("Thank you for playing World of Zuul!");
        }

        private void Move(string direction)
        {
            int newX = World.PlayerX;
            int newY = World.PlayerY;

            switch (direction)
            {
                case "south":
                    newX++;
                    break;
                case "north":
                    newX--;
                    break;
                case "west":
                    newY--;
                    break;
                case "east":
                    newY++;
                    break;
            }

            var target = World.RoomManager.GetRoom(newX, newY); // no var needed
            if (target == null || target.TileIdentifier == '-')
            {
                tui.WriteLine("You can't go that way!");
                return;
            }

            // update previous and current
            previousCoords = [World.PlayerX, World.PlayerY]; // !!!! this is not working
            World.PlayerX = newX;
            World.PlayerY = newY;
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
