using WorldOfZuul.Logic;

namespace UnitTests;

public class PlayerTests
{
    [Test]
    public void MovePlayerToDifferentRoom()
    {
        GameState world = new GameState(3, 3);
        world.Player.X = 0;
        world.Player.Y = 0;

        world.Player.Move("south");
        Assert.That(world.Player.X, Is.EqualTo(1), "Player X coordinate should be updated after moving south");
        Assert.That(world.Player.Y, Is.EqualTo(0), "Player Y coordinate should be updated after moving south");
    }
}