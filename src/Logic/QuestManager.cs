namespace WorldOfZuul.Logic;
public class QuestManager
{
    private readonly GameState World;
    internal Dictionary<string, Quest> Quests { get; set; } = [];

    public QuestManager(GameState _World)
    {
        World = _World;
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
    public void CheckCompletion(string questName, string ?interactingNpc = null)
    {
        foreach (var trigger in Quests[questName].CompletionTriggers)
        {
            if (trigger.Type == "move_item" && trigger.Room != null && trigger.Item != null)
            {
                var room = World.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                if (room != null && room.Items.Contains(trigger.Item))
                {
                    Quests[questName].State = "completed";
                    ApplyQuestCompletionActions(Quests[questName]);
                    return;
                }
            }
            else if (trigger.Type == "talk_to_npc" && interactingNpc != null && interactingNpc == trigger.Npc)
            {
                Quests[questName].State = "completed";
                ApplyQuestCompletionActions(Quests[questName]);
                return;
            } // else
            // {
            //     // Console.WriteLine("Error finishing the quest");
            //     return;
            // }
        }
    }

    private void ApplyQuestCompletionActions(Quest quest)
    {
        foreach (var action in quest.OnCompleteActions)
        {
            switch (action.Type)
            {
                case "move_npc":
                    if (action.Npc == null)
                    {
                        throw new Exception("Wrong action while trying to complete the quest "+quest.Title);
                    }
                    World.NPCManager.MoveNPC(action.Npc, action.ToX, action.ToY);
                    break;
                // case "spawn_item":
                //     SpawnItem(action.RoomX, action.RoomY, action.Item);
                //     break;
                case "give_item":
                    if (action.Item == null)
                    {
                        throw new Exception("Wrong action while trying to complete the quest "+quest.Title);
                    }
                    World.ItemManager.MoveToInventory(action.Item);
                    break;
                case "take_item":
                    if (action.Item == null)
                    {
                        throw new Exception("Wrong action while trying to complete the quest "+quest.Title);
                    }
                    World.ItemManager.MoveOutOfInventory(action.Item);
                    break;
            }
        }
    }
}
