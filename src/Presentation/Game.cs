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
                        foreach (string npcName in World.Player.CurrentRoom.NPCs)
                        {
                            NPC npc = World.NPCManager.GetNPC(npcName);
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

        private void TalkToNPC(string? secondCommandWord)
        {
            List<string> npcs = World.Player.CurrentRoom.NPCs;
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
                npc = World.NPCManager.GetNPC(npcs[0]);
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
                        npc = World.NPCManager.GetNPC(npcs[i]);
                        tui.WriteLine($"{i+1}. {npc.Name}");
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
                npc = World.NPCManager.GetNPC(npcs[npcNumber]);
            }

            // finally, talk to the NPC
            isInDialogue = true;
            // if there is no quest active
            if (World.Player.ActiveQuestName == "")
            {
                Quest? q = World.QuestManager.FindAvailableQuest(npc);
                // no availabe quests -> default dialogue
                if (q == null)
                {
                    tui.WriteLine($"{npc.Name}: Hi! How's your day goin'?");
                    return;
                }
                // if available quest is found -> preQuestDialogue + add 
                for (int i = 0; i < q.PreQuestDialogue.Count; i++)
                {
                    tui.WriteLine($"{npc.Name}: {q.PreQuestDialogue[i]}");
                    Console.ReadKey();
                }
                tui.WriteLine("");
                tui.WriteLine("Would you be down to do this?");
                Console.Write("> ");
                string? text = Console.ReadLine();

                if (text != null && text == "yes")
                {
                    q.State = "active";
                    World.Player.ActiveQuestName = q.Title;
                    tui.WriteLine($"Quest Accepted: {q.Title}");
                } else
                {
                    tui.WriteLine("Well, come back when you'll change your mind.");
                }
                return;
            }

            // if there is an active quest
            Quest quest = World.QuestManager.GetQuest(World.Player.ActiveQuestName);
            bool isCompleted = World.QuestManager.CheckCompletion(
                questName: World.Player.ActiveQuestName,
                interactingNpc: npc.Name
            );

            if (isCompleted)
            {
                for (int i = 0; i < quest.CompletionDialogue.Count; i++)
                {
                    tui.WriteLine($"{npc.Name}: {quest.CompletionDialogue[i]}");
                    Console.ReadKey();
                }
            } else
            {
                tui.WriteLine("You haven't met the criteria to finish this quest!");
            }
        }

        private void CatchMoveError(string? errorText)
        {
            if (errorText == null)
            {

                tui.WriteLine(World.Player.CurrentRoom.Description ?? "None");

                if (World.Player.CurrentRoom.NPCs.Count != 0) 
                    {
                        foreach (string npcName in World.Player.CurrentRoom.NPCs)
                        {
                            NPC npc = World.NPCManager.GetNPC(npcName);
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
