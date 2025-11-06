/* using System;
using System.Collections.Generic;
using System.Threading; */

using Spectre.Console;

namespace WorldOfZuul
{
    public class Game
    {
        // currentRoom: [row, col]
        private int[] currentRoom = new int[] { 2, 1 };
        private int[]? previousRoom = null;
        private TUI tui = new();

        public Game()
        {
            CreateRooms();
        }

        private void CreateRooms()
        {
            // Room map is provided by Room.Map in Room.cs
        }

        public void Play()
        {
            // temporary solution, fix later
            Parser parser = new();
            // Room[][] Rooms = [
            //     [new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground2.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground3.csv")],
            //     [new('O', "Room2", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground5.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv")],
            //     [new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv")],
            //     [new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv"), new('O', "Ocean", "You are in the middle of the vast ocean. The water stretches out in all directions, with no land in sight.", "roomBackground1.csv")]
            // ];

            Quest q2 = new("2nd quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            Quest q3 = new("Third quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            Quest q1 = new("First quest", "Test description", "Test ibjective", [[2, 1], [3, 4]]);
            QuestManager qManager = new([q1, q2, q3]);

            NPC oldGuy = new("NikolasKokkalis", "villager", "just a chill guy", ["Hello, im just a chill guy", "We have big problems here"], q2);
            Room.GetRoomAt(1, 1)?.SetNPC(oldGuy);
            // Rooms[2][1].SetNPC(oldGuy);

            PrintWelcome();
            tui.WaitForCorrectTerminalSize();

            bool continuePlaying = true;
            while (continuePlaying)
            {
                var current = Room.GetRoomAt(currentRoom[0], currentRoom[1]);
                tui.WriteLine(current?.ShortDescription ?? "None");
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
                        tui.WriteLine(current?.LongDescription ?? "Nothing to look at");
                        tui.DrawCanvas();
                        break;

                    case "back":
                        if (previousRoom == null)
                            tui.WriteLine("You can't go back from here!");
                        else
                            currentRoom = [previousRoom[0], previousRoom[1]];
                        break;

                    case "north":
                    case "south":
                    case "east":
                    case "west":
                        Move(command.Name);
                        var newRoom = Room.GetRoomAt(currentRoom[0], currentRoom[1]);
                        if (newRoom != null)
                            tui.UpdateBackground(newRoom);
                        break;

                    case "talk":
                        var r = Room.GetRoomAt(currentRoom[0], currentRoom[1]);
                        if (r == null || r.RoomNPC == null)
                        {
                            tui.WriteLine("No one is here!");
                            break;
                        }

                        if (r.RoomNPC.quest != null && r.RoomNPC.quest.State == QuestState.Pending)
                        {
                            r.RoomNPC.Talk();
                        }
                        else
                        {
                            tui.WriteLine("no quest sorry");
                            //currentRoom.RoomNPC.Dialogue1();
                        }
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
            int newRow = currentRoom[0];
            int newCol = currentRoom[1];

            switch (direction)
            {
                case "south":
                    newRow++;
                    break;
                case "north":
                    newRow--;
                    break;
                case "west":
                    newCol--;
                    break;
                case "east":
                    newCol++;
                    break;
            }

            var target = Room.GetRoomAt(newRow, newCol);
            if (target == null)
            {
                tui.WriteLine($"You can't go that way!");
                return;
            }

            // update previous and current
            previousRoom = new int[] { currentRoom[0], currentRoom[1] };
            currentRoom[0] = newRow;
            currentRoom[1] = newCol;
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
