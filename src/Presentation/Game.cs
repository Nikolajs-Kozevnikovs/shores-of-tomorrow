namespace WorldOfZuul.Presentation
{
    using WorldOfZuul.Logic;

    public class Game
    {
        private readonly TUI tui = new();
        private readonly GameState World = new(10, 10);
        private readonly RandomEvents randomEvents = new();
        private bool isInDialogue = false;
        
        
        public void Play()
        {
            Parser parser = new();

            tui.WaitForCorrectTerminalSize();

            LoadSave.ChooseSave(tui, World);

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
            tui.ClearDialogBoxLines();
            switch (command.Name)
            {
                case "look":
                    tui.WriteLine(World.Player.CurrentRoom.Description ?? "Nothing to look at (devs forgot a description)");
                    if (World.Player.CurrentRoom.NPCs.Count != 0) 
                    {
                        foreach (NPC npc in World.Player.CurrentRoom.NPCs)
                        {
                            tui.WriteLine($"You see {npc.Name}, {npc.Profession}");
                        }
                    }
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
                    TalkToNPC(command.SecondWord);
                    break;
                case "take":
                    TakeItem(command.SecondWord);
                    break;
                case "fish":
                    Fishing();
                    break;
                case "inventory":
                    CheckInventory();
                    break;
                case "help":
                    PrintHelp();
                    break;
                case "save":
                    LoadSave.SaveProgress(tui, World);
                    break;
                case "quit":
                    tui.WriteLine("Thank you for playing World of Zuul!");
                    LoadSave.SaveProgress(tui, World);
                    return false;

                default:
                    tui.WriteLine("I don't know what command.");
                    break;
            }

            return true;
        }

        private void Fishing()
        {
            // // net route (1)
            // "You prepared a net and threw it into water.",
            // "10 minutes have passed, the net is out.",
            // "You caught 25 fish! Add to the inventory? Choice: yes/no.",
            
            // // rods route (2)
            // "You prepared rods and started fishing.",
            // "10 minutes passed.",
            // "You've got a fish! Add to the inventory? Choice: yes/no.",
            // "You've got a fish! Add to the inventory? Choice: yes/no.",
            // "No more luck today..."
        }

        private void CheckInventory()
        {
            tui.WriteLine("You look into your bags.");
            tui.WriteLine("");
            for (int i = 0; i < World.Player.Inventory.Count; i++)
            {
                tui.WriteLine($"{i+1}. {World.Player.Inventory[i].Name}");
            }
        }

        private void TakeItem(string? secondCommandWord)
        {
            List<Item> items = World.Player.CurrentRoom.Items;

            if (items.Count == 0)
            {
                tui.WriteLine("Nothing to take in this room!");
                return;
            }

            if (items.Count == 1)
            {
                MoveItem(items[0], World.Player.CurrentRoom, World.Player);
                tui.WriteLine($"You took {items[0]} to your inventory.");
                return;
            }


            if (secondCommandWord == null) 
            {
                tui.WriteLine("There are multiple items in this room! To choose an itemm use 'take [number]'");
                tui.WriteLine("Available items:");
                for (int i = 0; i < items.Count; i++)
                {
                    tui.WriteLine($"{i+1}. {items[i].Name}");
                }
                return;
            }
            try {
                int n = int.Parse(secondCommandWord);
                MoveItem(items[n], World.Player.CurrentRoom, World.Player);
                tui.WriteLine($"You took {items[n]} to your inventory.");
                return;
            } catch(Exception)
            {
                tui.WriteLine("Wrong input");
                return;
            }

        }

        private void TalkToNPC(string? secondCommandWord)
        {
            List<NPC> npcs = World.Player.CurrentRoom.NPCs;
            // if there are no NPCs
            if (npcs.Count == 0)
            {
                isInDialogue = false;
                tui.WriteLine("No one is here!");
                return;
            }

            NPC? npc;
            // if there is one NPC
            if (npcs.Count == 1)
            {
                isInDialogue = true;
                npc = npcs[0];
            }
            // if there are multiple NPCs
            else
            {
                // if user doesn't specify which NPC to talk to
                if (secondCommandWord == null)
                {
                    tui.WriteLine("There are multiple NPCs in this room! To choose NPC to talk to, use 'talk [number]'");
                    tui.WriteLine("Available NPCs:");
                    for (int i = 0; i < npcs.Count; i++)
                    {
                        tui.WriteLine($"{i+1}. {npcs[i].Name}");
                    }
                    return;
                }
                // if user specifies which NPC to talk to, check if the number is in range
                int npcNumber = int.Parse(secondCommandWord) - 1;

                if (npcNumber > npcs.Count)
                {
                    tui.WriteLine("Number is out of range of available NPCs");
                    tui.WriteLine("To check available NPCs in this Room, type 'talk'");
                }
                npc = World.Player.CurrentRoom.NPCs[npcNumber];
            }

            // finally, talk to the NPC
            isInDialogue = true;
            // if there is no quest active
            if (World.Player.QuestProgression.ActiveQuests.Count == 0)
            {
                World.Player.QuestProgression.TryAcceptQuest(npc, tui);
                return;
            }

            // // if there is an active quest
            World.Player.QuestProgression.TryFinishQuest(npc, tui, World);
        }
        

        private void CatchMoveError(string? errorText)
        {
            if (errorText == null)
            {

                tui.WriteLine(World.Player.CurrentRoom.Description ?? "None");

                if (World.Player.CurrentRoom.NPCs.Count != 0) 
                    {
                        foreach (NPC npc in World.Player.CurrentRoom.NPCs)
                        {
                            tui.WriteLine($"You see {npc.Name}, {npc.Profession}");
                        }
                    }
                tui.UpdateBackground(World.Player.CurrentRoom);
                return;
            }

            tui.WriteLine(errorText);
        }       

        public static void MoveItem(Item item, IItemContainer from, IItemContainer to)
        {
            if (from.RemoveItem(item))
                to.AddItem(item);
            else
                Console.WriteLine("Item not found in source container.");
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
