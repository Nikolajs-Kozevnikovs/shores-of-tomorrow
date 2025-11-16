public class NPCManager
{
    private GameState world { get; }
    public Dictionary<string, NPC> Npcs { get; set; } = [];

    public NPCManager(GameState _world)
    {
        world = _world;
    }

    public void MoveNpc(string npcName, int newX, int newY)
    {
        if (Npcs.TryGetValue(npcName, out NPC npc))
        {
            world.RoomManager.RemoveNpcFromRoom(npcName, npc.X, npc.Y);
            npc.X = newX;
            npc.Y = newY;
            world.RoomManager.AddNpcToRoom(npcName, newX, newY);
        }
    }
}
