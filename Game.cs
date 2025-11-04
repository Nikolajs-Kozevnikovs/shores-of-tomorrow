namespace WorldOfZuul
{
    public class Game
    {
        private Room? currentRoom;
        private Room? previousRoom;
        // private List<Quest> activeQuests;

        public Game()
        {
            CreateRooms();
        }

        private void CreateRooms()
        {
            Room? outside = new("Outside", "You are standing outside the main entrance of the university. To the east is a large building, to the south is a computing lab, and to the west is the campus pub.", "roomBackground1.txt");
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
            Quest q2 = new("2nd quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            Quest q3 = new("Third quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            Quest q1 = new("First quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            QuestManager qManager = new([ q1, q2, q3]);

            NPC oldGuy = new NPC("NikolasKokkalis", "villager", "just a chill guy", ["Hello, im just a chill guy", "We have big problems here"], q2);
            outside.SetNPC(oldGuy);
        }

        public void Play()
        {
            Parser parser = new();

            PrintWelcome();
            // temporary solution for window size
            while (true)
            {
                // Get the current terminal window size
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                // Check if the terminal size is less than 132x43
                if (width < 132 || height < 43)
                {
                    Console.Clear(); // Clear the console for better readability
                    Console.WriteLine("Terminal too small. Please increase the size.");
                }
                else
                {
                    Console.Clear(); // Clear the console before starting the game
                    break; // Exit the loop after starting the game
                }

                // Sleep for a while before rechecking
                Thread.Sleep(1000); // Check every second
            }

            TUI tui = new();

            bool continuePlaying = true;
            while (continuePlaying)
            {

                Console.WriteLine(currentRoom?.ShortDescription);
                Console.Write("> ");

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Please enter a command.");
                    tui.DrawCanvas();
                    continue;
                }

                Command? command = parser.GetCommand(input);

                if (command == null)
                {
                    Console.WriteLine("I don't know that command.");
                    tui.DrawCanvas();
                    continue;
                }

                switch (command.Name)
                {
                    case "look":
                        Console.WriteLine(currentRoom?.LongDescription);
                        break;

                    case "back":
                        if (previousRoom == null)
                            Console.WriteLine("You can't go back from here!");
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

                    case "talk":
                        if (currentRoom.RoomNPC == null) {
                            Console.WriteLine("No one is here!");
                            break;
                        }
                        
                        if ( currentRoom.RoomNPC.quest != null && currentRoom.RoomNPC.quest.State == QuestState.Pending) {
                            currentRoom.RoomNPC.Talk();
                        } else {
                            Console.WriteLine("no quest sorry");
                            //currentRoom.RoomNPC.Dialogue1();
                        }
                        break;

                    case "quit":
                        continuePlaying = false;
                        break;

                    case "help":
                        PrintHelp();
                        break;

                    default:
                        Console.WriteLine("I don't know what command.");
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
                Console.WriteLine($"You can't go {direction}!");
            }
        }


        private static void PrintWelcome()
        {
            Console.WriteLine("Welcome to the World of Zuul!");
            Console.WriteLine("World of Zuul is a new, incredibly boring adventure game.");
            PrintHelp();
            Console.WriteLine();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("You are lost. You are alone. You wander");
            Console.WriteLine("around the university.");
            Console.WriteLine();
            Console.WriteLine("Navigate by typing 'north', 'south', 'east', or 'west'.");
            Console.WriteLine("Type 'look' for more details.");
            Console.WriteLine("Type 'back' to go to the previous room.");
            Console.WriteLine("Type 'help' to print this message again.");
            Console.WriteLine("Type 'quit' to exit the game.");
        }
    }
}
