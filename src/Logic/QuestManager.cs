public class QuestManager
{
    private GameState world { get; }
    public Dictionary<string, Quest> Quests { get; set; } = [];

    public QuestManager(GameState _world)
    {
        world = _world;
    }

    public void UpdateQuestVisibility()
    {
        foreach (var quest in Quests.Values)
        {
            if (quest.State == "not_started")
            {
                bool visible = true;
                foreach (var condition in quest.VisibilityConditions)
                {
                    foreach (string QuestId in condition.CompletedQuestIds)
                    {
                        if (Quests[QuestId].State != "completed")
                        {
                            visible = false;
                            break;
                        }
                    }
                }
                if (visible) quest.State = "available";
            }
        }
    }
    // I'd love to have a GameState here
    // also need a try for null references
    public void CheckCompletion(string questName, string interactingNpc = null)
    {
        foreach (var trigger in Quests[questName].CompletionTriggers)
        {
            if (trigger.Type == "move_item")
            {
                var room = world.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                if (room.Items.Contains(trigger.Item))
                {
                    Quests[questName].State = "completed";
                    return;
                }
            }
            else if (trigger.Type == "talk_to_npc" && interactingNpc == trigger.Npc)
            {
                Quests[questName].State = "completed";
                return;
            }
        }
    }
}
