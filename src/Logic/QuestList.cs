namespace WorldOfZuul.Logic;

public static class QuestList
{
    private static readonly Dictionary<string, Quest> _quests = new();


    public static void Register(Quest quest)
    {
        if (quest == null) throw new ArgumentNullException(nameof(quest));
        if (_quests.ContainsKey(quest.Title))
            throw new Exception($"Quest with ID '{quest.Title}' is already registered.");
        _quests[quest.Title] = quest;
    }

    public static Quest Get(string questId)
    {
        if (!_quests.TryGetValue(questId, out var quest))
            throw new Exception($"Quest '{questId}' not found in registry.");
        return quest;
    }

    public static IReadOnlyCollection<Quest> All => _quests.Values;
}
