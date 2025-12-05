namespace WorldOfZuul.Logic;
public class NPCManager
{
    private readonly GameState World;
    internal Dictionary<string, NPC> Npcs { get; set; } = [];

    public NPCManager(GameState _World)
    {
        World = _World;
    }

    public void MoveNpc(string npcName, int newX, int newY)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        if (Npcs.TryGetValue(npcName, out NPC npc))
        {
            World.RoomManager.RemoveNpcFromRoom(npcName, npc.X, npc.Y);
            npc.X = newX;
            npc.Y = newY;
            World.RoomManager.AddNpcToRoom(npcName, newX, newY);
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }
}
