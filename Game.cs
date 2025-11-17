/* using System;
using System.Collections.Generic;
using System.Threading; */

using Namotion.Reflection;
using Spectre.Console;

namespace WorldOfZuul
{
    public class Game
    {
        // currentRoom: [row, col]
        private int[]? previousCoords = null;
        private readonly TUI tui = new();
        private readonly GameState world = new(7, 7);

        public Game()
        {
            // load things from json
            GameStateLoader.Load(world, "/");
        }
        
        public void Play()
        {
            Parser parser = new();   

            PrintWelcome();
            tui.WaitForCorrectTerminalSize();
            tui.DrawCanvas();
            
            var currentRoom = world.RoomManager.GetCurrentRoom();
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

                    case "back": // not working!
                        if (previousCoords == null)
                        {
                            tui.WriteLine("You can't go back from here!");
                        } else
                        {
                            world.PlayerX = previousCoords[0];
                            world.PlayerY = previousCoords[1];
                            previousCoords = null;
                            currentRoom = world.RoomManager.GetCurrentRoom();
                            tui.WriteLine(currentRoom.Description ?? "None");
                            tui.UpdateBackground(currentRoom);
                        }
                        break;

                    case "north":
                    case "south":
                    case "east":
                    case "west":
                        Move(command.Name); // doesn't change current room
                        currentRoom = world.RoomManager.GetCurrentRoom();
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
            int newX = world.PlayerX;
            int newY = world.PlayerY;

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

            var target = world.RoomManager.GetRoom(newX, newY);
            if (target == null || target.TileIdentifier == '-')
            {
                tui.WriteLine("You can't go that way!");
                return;
            }

            // update previous and current
            previousCoords = [world.PlayerX, world.PlayerY]; // !!!! this is not working
            world.PlayerX = newX;
            world.PlayerY = newY;
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
