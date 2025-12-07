using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;

public class Player
{
    private readonly GameState World; 
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int[]? PreviousCoords { get; set; }

    public Player(int x, int y, GameState world)
    {
        X = x;
        Y = y;
        World = world;
    }
    
    public void Move(string direction, TUI tui)
    {
        int newX = X;
        int newY = Y;

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

        Room? target = World.RoomManager.GetRoom(newX, newY);
        if (target == null || target.TileIdentifier == '-')
        {
            tui.WriteLine("You can't go that way!");
            return;
        }

        PreviousCoords = [X, Y];
        X = newX;
        Y = newY;
    }

    public void Back(TUI tui)
    {
        if (PreviousCoords == null)
        {
            tui.WriteLine("You can't go back from here!");
            return; 
        }

        X = PreviousCoords[0];
        Y = PreviousCoords[1];

        PreviousCoords = null;
    }
}