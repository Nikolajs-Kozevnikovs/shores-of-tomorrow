using SixLabors.ImageSharp.Processing.Processors.Convolution;
using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;
public class QuestManager
{
    private readonly GameState World;
    public  Dictionary<string, Quest> Quests { get; set; } = [];

    public QuestManager(GameState _World)
    {
        World = _World;
    }

    public void UpdateQuestVisibility()
    {
        foreach (var quest in Quests.Values)
        {
            if (quest.State == "locked")
            {
                bool visible = true;
                foreach (var questCompleted in quest.VisibilityConditions)
                {
                    if (Quests[questCompleted].State != "completed")
                    {
                        visible = false;
                        break;
                    }
                    
                }
                if (visible) quest.State = "available";
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
            if (trigger.Type == "move_item" && trigger.Room != null && trigger.Item != null)
            {
                var room = World.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                if (room != null && room.Items.Contains(trigger.Item))
                {
                    quest.State = "completed";
                    ApplyQuestCompletionActions(Quests[questName]);
                    UpdateQuestVisibility();
                    World.Player.ActiveQuestName = null;
                    return true;
                }
            }
            else if (trigger.Type == "talk_to_npc" && interactingNpc != null && interactingNpc == trigger.Npc)
            {
                quest.State = "completed";
                ApplyQuestCompletionActions(Quests[questName]);
                UpdateQuestVisibility();
                World.Player.ActiveQuestName = null;
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
