namespace WorldOfZuul.Logic;

public static class NPCRegistry
{
    private static readonly Dictionary<string, NPC> _definitions = new();

    public static void Register(NPC npc)
    {
        _definitions[npc.Name] = npc;
    }
    // CreateNPC() doesn't create a new instance of an NPC, it only returns the same one. This means that if I add NPC to two rooms, it will be the same NPC
    public static NPC CreateNPC(string name)
    {
        if (!_definitions.TryGetValue(name, out var npc))
            throw new Exception($"NPC '{name}' not found in registry");

        return npc;
    }
}
