namespace WorldOfZuul.Presentation
{
    using WorldOfZuul.Logic;

    public class Game
    {
        private readonly GameState World = new(10, 10);
        private readonly RandomEvents randomEvents = new();
        private bool isInDialogue = false;


        public void Play()
        {
            TUI.StartGame();
            TUI.CurrentWorld = World; // give TUI access to the GameState

            Parser parser = new();

            TUI.WaitForCorrectTerminalSize();

            LoadSave.ChooseSave(World);

            TUI.UpdateBackground(World.Player.CurrentRoom);

            TUI.WriteLine(World.Player.CurrentRoom.Description ?? "None");
            PrintWelcome();

            TUI.DrawCanvas();

            bool continuePlaying = true;
            while (continuePlaying)
            {
                continuePlaying = ProcessUserInput(parser);
                if (continuePlaying)
                {
                    randomEvents.TryTrigger(World.Player.CurrentRoom, isInDialogue);
                }
                TUI.DrawCanvas();
            }
        }

        private bool ProcessUserInput(Parser parser)
        {
            Console.Write("> ");

            string? input = Console.ReadLine();
            // check for empty input
            if (string.IsNullOrEmpty(input))
            {
                TUI.WriteLine("Please enter a command.");
                return true;
            }
            // parse into command
            Command? command = parser.GetCommand(input);
            // check if command is invalid
            if (command == null || command.Name == null)
            {
                TUI.WriteLine("I don't know that command.");
                return true;
            }
            // execute the command
            return HandleCommand(command);
        }

        private bool HandleCommand(Command command)
        {
            TUI.ClearDialogBoxLines();
            switch (command.Name)
            {
                case "look":
                    TUI.WriteLine(World.Player.CurrentRoom.Description ?? "Nothing to look at (devs forgot a description)");
                    if (World.Player.CurrentRoom.NPCs.Count != 0)
                    {
                        foreach (NPC npc in World.Player.CurrentRoom.NPCs)
                        {
                            TUI.WriteLine($"You see {npc.Name}, {npc.Profession}");
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
                    CatchMoveError(World.Player.Move(command.Name, command.SecondWord));
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
                case "quest":
                    if (World.Player.QuestProgression.ActiveQuests.Count != 0)
                    {
                        foreach (string qId in World.Player.QuestProgression.ActiveQuests)
                        {
                            TUI.WriteLine($" - {QuestList.Get(qId).Description}");
                        }
                    } else
                    {
                        TUI.WriteLine("No active quests!");
                    }
                    break;
                case "sell":
                    if (World.Player.CurrentRoom.Name != "Fish shop")
                    {
                        TUI.WriteLine("You can't sell anything here!");
                    } else
                    {
                        if (RemoveFish())
                        {
                            TUI.WriteLine("You got a hefty bag of cash for your efforts.");
                            Item cash = ItemRegistry.CreateItem("cash");
                            World.Player.AddItem(cash);
                        } else
                        {
                            
                        }
                        
                    }
                    break;
                case "drop":
                    DropItem(command.SecondWord);
                    break;
                case "donate":
                    if (World.Player.CurrentRoom.Name != "Food charity")
                    {
                        TUI.WriteLine("You can't donate anything here!");
                    } else {
                        if (RemoveFish())
                        {
                            Item letter = ItemRegistry.CreateItem("letter");
                            World.Player.AddItem(letter);     
                            TUI.WriteLine("You got a Thank-you letter.");
                        }
                        
                    }
                    break;
                case "help":
                    PrintHelp();
                    break;
                case "save":
                    LoadSave.SaveProgress(World);
                    break;
                case "quit":
                    TUI.WriteLine("Thank you for playing World of Zuul!");
                    LoadSave.SaveProgress(World);
                    return false;

                default:
                    TUI.WriteLine("I don't know what command.");
                    break;
            }

            return true;
        }

        private void DropItem(string? secondCommandWord)
        
        {
            if (World.Player.Inventory.Count == 0)
            {
                TUI.WriteLine("Nothing to drop!");
                return;
            }

            if (secondCommandWord == null || secondCommandWord == "") 
            {
                TUI.WriteLine("There are multiple items in this room! To choose an item use 'take <number>'");
                TUI.WriteLine("Available items:");
                for (int i = 0; i < World.Player.Inventory.Count; i++)
                {
                    TUI.WriteLine($"{i+1}. {World.Player.Inventory[i].Name}");
                }
                return;
            }
            try {
                int n = int.Parse(secondCommandWord);
                Item item = MoveItem(World.Player.Inventory[n-1], World.Player.Inventory, World.Player.CurrentRoom.Items);
                TUI.WriteLine($"You put {item.Name} to the ground.");
                return;
            } catch(Exception)
            {
                TUI.WriteLine("Wrong input");
                return;
            }
        }
        // complicated, but it works
        private void Fishing()
        {
            Room currentLocation = World.Player.CurrentRoom;
            Item? tool = null;
            
            Dictionary<string, double> odds = new(){
                { "fishing_pole", 0.6 },
                { "fishing_net", 0.8 },
                { "fishing_bomb", 0.8 }
            };
            switch(currentLocation.Name)
            {
                case "Fishing Boat":
                    // prevent fishing with two tools
                    tool = World.Player.Inventory.Find(i => i.Id == "fishing_pole");
                    Item? tool2 = World.Player.Inventory.Find(i => i.Id == "fishing_net");
                    if (tool != null && tool2 != null)
                    {
                        TUI.WriteLine("You have multiple tools to fish with: fishing rod (1) and fishing net (2). Choose one by typing a number.");
                        string? number = Console.ReadLine();
                        if (number != null && number != "")
                        {
                            try
                            {
                                int num = int.Parse(number);
                                if (num == 2)
                                {
                                    tool = tool2;
                                } else if (num != 1)
                                {
                                    TUI.WriteLine("Wrong input!");
                                    return;
                                }
                            } catch (Exception)
                            {
                                TUI.WriteLine("Wrong input!");
                                return;
                            }
                        }
                        // return;
                    }
                    break;
                case "Coral Reef":
                    tool = World.Player.Inventory.Find(i => i.Id == "fishing_bomb");
                    break;
                case "Ocean":
                case "Remote Ocean":
                    tool = World.Player.Inventory.Find(i => i.Id == "fishing_pole");
                    break;
                default:
                    TUI.WriteLine("You can't fish here!");
                    break;
            }
            if (tool == null) {
                TUI.WriteLine("You don't have anything to fish with!");
                return;
            }

            

            TUI.WriteLine("Fishing...");
            // fishing minigame
            TUI.WriteLine($"Pick a number between 1 and 5");
            string? picked = Console.ReadLine();
            if (picked == null || picked == "")
            {
                TUI.WriteLine("Wrong input");
                return;
            }
            try
            {
                int pick = int.Parse(picked);
                List<bool> tiles = new List<bool>();
                Random rng = new Random();

                for (int i = 0; i < 5; i++)
                {
                    tiles.Add(rng.NextDouble() < odds[tool.Id]);
                }
                // TUI.WriteLine("We got to this point");
                if (tiles[pick-1])
                {
                    Dictionary<string, string> fish_type = new(){
                        { "Ocean", "close_waters_fish" },
                        { "Remote Ocean", "remote_waters_fish" },
                        { "Coral Reef", "rare_fish" },
                        {"Fishing Boat", "fish_caught_with_" }
                    };
                    string fish_id = fish_type[currentLocation.Name];
                    if (currentLocation.Name == "Fishing Boat")
                    {
                        if (tool.Id == "fishing_pole")
                        {
                            fish_id += "pole";
                        } else
                        {
                            fish_id += "net";
                        }
                    }
                    Item fish = ItemRegistry.CreateItem(fish_id);
                    World.Player.AddItem(fish);
                    TUI.WriteLine($"Gotcha! You caught {fish.Name}");
                    if (tool.Id == "fishing_net")
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            fish = ItemRegistry.CreateItem(fish_id);
                            World.Player.AddItem(fish);
                            TUI.WriteLine($"Gotcha! You caught {fish.Name}");
                        }
                    }
                } else
                {
                    TUI.WriteLine("No luck this time, try again!");
                }
            } catch (Exception)
            {
                TUI.WriteLine("Wrong input!");
                TUI.WriteLine("Or the code is wrong");
                return;
            }
            
            
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
            TUI.WriteLine("You look into your bags.");
            TUI.WriteLine("");
            for (int i = 0; i < World.Player.Inventory.Count; i++)
            {
                TUI.WriteLine($"{i+1}. {World.Player.Inventory[i].Name}");
            }
        }

        private void TakeItem(string? secondCommandWord)
        {
            List<Item> items = World.Player.CurrentRoom.Items;

            if (items.Count == 0)
            {
                TUI.WriteLine("Nothing to take in this room!");
                return;
            }

            if (items.Count == 1)
            {
                Item item = MoveItem(items[0], World.Player.CurrentRoom.Items, World.Player.Inventory);
                TUI.WriteLine($"You took {item.Name} to your inventory.");
                return;
            }


            if (secondCommandWord == null) 
            {
                TUI.WriteLine("There are multiple items in this room! To choose an item use 'take <number>'");
                TUI.WriteLine("Available items:");
                for (int i = 0; i < items.Count; i++)
                {
                    TUI.WriteLine($"{i+1}. {items[i].Name}");
                }
                return;
            }
            try {
                int n = int.Parse(secondCommandWord);
                if (n > 2)
                {
                    throw new Exception("Wrong input");
                }
                TUI.WriteLine($"You took {items[n-1].Name} to your inventory.");
                MoveItem(items[n-1], World.Player.CurrentRoom.Items, World.Player.Inventory);
                // return;
            } catch(Exception)
            {
                TUI.WriteLine("Wrong input");
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
                TUI.WriteLine("No one is here!");
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
                    TUI.WriteLine("There are multiple NPCs in this room! To choose NPC to talk to, use 'talk [number]'");
                    TUI.WriteLine("Available NPCs:");
                    for (int i = 0; i < npcs.Count; i++)
                    {
                        TUI.WriteLine($"{i+1}. {npcs[i].Name}");
                    }
                    return;
                }
                // if user specifies which NPC to talk to, check if the number is in range
                int npcNumber = int.Parse(secondCommandWord) - 1;

                if (npcNumber > npcs.Count)
                {
                    TUI.WriteLine("Number is out of range of available NPCs");
                    TUI.WriteLine("To check available NPCs in this Room, type 'talk'");
                }
                npc = World.Player.CurrentRoom.NPCs[npcNumber];
            }

            // finally, talk to the NPC
            isInDialogue = true;
            bool isFinished = World.Player.QuestProgression.TryFinishQuest(npc, World);
            bool isAvailable = World.Player.QuestProgression.TryAcceptQuest(npc);
            if (!isFinished && !isAvailable)
            {
                TUI.WriteLine($"{npc.Name}: Hi! How's it going?");
            }
        }
        

        private void CatchMoveError(string? errorText)
        {
            if (errorText == null)
            {

                TUI.WriteLine(World.Player.CurrentRoom.Description ?? "None");

                if (World.Player.CurrentRoom.NPCs.Count != 0)
                    {
                        foreach (NPC npc in World.Player.CurrentRoom.NPCs)
                        {
                            TUI.WriteLine($"You see {npc.Name}, {npc.Profession}");
                        }
                    }
                TUI.UpdateBackground(World.Player.CurrentRoom);
                return;
            }

            TUI.WriteLine(errorText);
        }

        public static Item MoveItem(Item item, List<Item> from, List<Item> to)
        {
            if (from.Remove(item))
                to.Add(item);
            else
                Console.WriteLine("Item not found in source container.");

            return item;
        }

        private bool RemoveFish()
        {
            if (World.Player.Inventory.Where(item => item.Id == "rare_fish" || item.Id == "remote_waters_fish"|| item.Id == "close_waters_fish"|| item.Id == "fish_caught_with_net" || item.Id == "fish_caught_with_pole").ToList().Count != 0)
            {
                World.Player.RemoveItem("rare_fish", 999);
                World.Player.RemoveItem("remote_waters_fish", 999);
                World.Player.RemoveItem("close_waters_fish", 999);
                World.Player.RemoveItem("fish_caught_with_net", 999);
                World.Player.RemoveItem("fish_caught_with_pole", 999);
                return true;
            } else
            {
                TUI.WriteLine("You don't have any fish!");
                return false;
            }
        }


        private void PrintWelcome()
        {
            TUI.WriteLine("Welcome to Shores of Tomorrow!");
            TUI.WriteLine("");
            TUI.WriteLine("");
            TUI.WriteLine("This is an open-world adventure game.");
            TUI.WriteLine("Feel free to roam around whenever you see something interesting or the main story gets boring.");
            TUI.WriteLine("If you don't know what to do at any point, type 'help' to get available commands.");
            TUI.WriteLine("");
            TUI.WriteLine("To get started, type 'talk' to start the storyline.");
            
        }

        private void PrintHelp()
        {
            TUI.WriteLine("---------- Help Menu ---------");
            TUI.WriteLine("Navigate by typing 'north', 'south', 'east', or 'west'.");
            TUI.WriteLine("Type 'look' for more details.");
            TUI.WriteLine("Type 'back' to go to the previous room.");
            TUI.WriteLine("Type 'talk' to talk to an NPC");
            TUI.WriteLine("Type 'take' to pick up an item");
            TUI.WriteLine("Type 'fish' to try fishing.");
            TUI.WriteLine("Type 'quest' to get the active quest description");
            TUI.WriteLine("Type 'help' to print this message again.");
            TUI.WriteLine("Type 'quit' to exit the game.");

        }
    }
}
