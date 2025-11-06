namespace WorldOfZuul
{
    public class Game
    {
        private Room? currentRoom;
        private Room? previousRoom;
        private TUI tui = new();
        // private List<Quest> activeQuests;

        public Game()
        {
            CreateRooms();
        }

        private void CreateRooms()
        {
            Room? outside = new("Outside", "You are standing outside the main entrance of the university\n. To the east is a large building, to the south is a computi\nng lab, and to the west is the campus pub.", "roomBackground1.txt");
            Room? theatre = new("Theatre", "You find yourself inside a large lecture theatre. Rows of seats ascend up to the back, and there's a podium at the front. It's quite dark and quiet.", "roomBackground2.txt");
            Room? pub = new("Pub", "You've entered the campus pub. It's a cozy place, with a few students chatting over drinks. There's a bar near you and some pool tables at the far end.", "roomBackground3.txt");
            Room? lab = new("Lab", "You're in a computing lab. Desks with computers line the walls, and there's an office to the east. The hum of machines fills the room.", "roomBackground4.txt");
            Room? office = new("Office", "You've entered what seems to be an administration office. There's a large desk with a computer on it, and some bookshelves lining one wall.", "roomBackground5.txt");

            outside.SetExits(null, theatre, lab, pub); // North, East, South, West
            theatre.SetExit("west", outside);
            pub.SetExit("east", outside);
            lab.SetExits(outside, office, null, null);
            office.SetExit("west", lab);

            currentRoom = outside;
        }

        public void Play()
        {
            Parser parser = new();

            PrintWelcome();
            tui.WaitForCorrectTerminalSize();

            bool continuePlaying = true;
            while (continuePlaying)
            {
                // use tui.Clear() to clear the dialog box
                tui.WriteLine(currentRoom?.ShortDescription);
                tui.DrawCanvas();
                Console.Write("> ");

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    tui.WriteLine("Please enter a command.");
                    tui.DrawCanvas();
                    continue;
                }

                Command? command = parser.GetCommand(input);

                if (command == null)
                {
                    tui.WriteLine("I don't know that command.");
                    tui.DrawCanvas();
                    continue;
                }

                switch (command.Name)
                {
                    case "look":
                        tui.WriteLine(currentRoom?.LongDescription);
                        tui.DrawCanvas();
                        break;

                    case "back":
                        if (previousRoom == null)
                            tui.WriteLine("You can't go back from here!");
                        else
                            currentRoom = previousRoom;
                        break;

                    case "north":
                    case "south":
                    case "east":
                    case "west":
                        Move(command.Name);
                        // this warning will get fixed as soon as we change the rooms structure
                        tui.UpdateBackground(currentRoom);
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

            Console.WriteLine("Thank you for playing World of Zuul!");
        }

        private void Move(string direction)
        {
            if (currentRoom?.Exits.ContainsKey(direction) == true)
            {
                previousRoom = currentRoom;
                currentRoom = currentRoom?.Exits[direction];
            }
            else
            {
                tui.WriteLine($"You can't go {direction}!");
            }
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
