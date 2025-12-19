namespace WorldOfZuul.Logic;
public class QuestManager
{
    private readonly GameState World;
    public  Dictionary<string, Quest> Quests { get; set; } = [];

    public QuestManager(GameState world)
    {
        World = world;
    }

    public void UpdateQuestVisibility(Quest q)
    {
        foreach (var questKey in Quests.Keys)
        {
            Quest quest = Quests[questKey];
            if (quest.State == "locked")
            {
                Console.WriteLine($"checking quest {quest.Title}");
                bool visible = true;
                foreach (var questCompleted in quest.VisibilityConditions)
                {
                    if (Quests[questCompleted].State != "completed")
                    {
                        visible = false;
                        break;
                    }
                    
                }
                if (visible) 
                    Quests[questKey].State = "available";
            }
        }
    }
    public bool CheckCompletion(string questName, string ?interactingNpc)
    {
        Quest quest = Quests[questName];

        if ( quest.State != "active")
            return false;

        foreach (var trigger in quest.CompletionTriggers)
        {
            if (trigger.Type == "move_item" && trigger.Room != null && trigger.ItemId != null)
            {
                var room = World.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                if (room != null && room.IsInside(trigger.ItemId))
                {
                    quest.State = "completed";
                    ApplyQuestCompletionActions(Quests[questName]);
                    UpdateQuestVisibility(quest);
                    World.Player.ActiveQuestName = "";
                    return true;
                }
            }
            else if (trigger.Type == "talk_to_npc" && interactingNpc != null && interactingNpc == trigger.Npc)
            {
                quest.State = "completed";
                ApplyQuestCompletionActions(Quests[questName]);
                UpdateQuestVisibility(quest);
                World.Player.ActiveQuestName = "";
                return true;
            } 
        }

        return false;
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
                case "get_item":
                    if (action.ItemId == null)
                    {
                        throw new Exception("Wrong action while trying to complete the quest "+quest.Title);
                    }
                    Item item = ItemRegistry.CreateItem(action.ItemId);
                    World.Player.AddItem(item);
                    break;
            }
        }
    }

    public Quest? FindAvailableQuest(NPC npc)
    {
        foreach (string questName in npc.QuestsGiven)
        {
            if (Quests[questName].State == "available")
            {
                return Quests[questName];
            }
        }
        return null;
    }

    public Quest GetQuest(string questName)
    {
        return Quests[questName];
    }
}
