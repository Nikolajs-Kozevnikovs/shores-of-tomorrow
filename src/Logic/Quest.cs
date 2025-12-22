using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;
public class Quest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string GiverNPC { get; set; } = "";
    public List<string> PreQuestDialogue { get; set; } = [];
    public Dictionary<string, List<string>> CompletionDialogue { get; set; } = [];
    public List<CompletionTrigger> CompletionTriggers { get; set; } = [];
    public List<OnFinishAction> OnFinishActions { get; set; } = [];

    public Quest()
    {
    }

    public CompletionTrigger? FindCompletionTrigger(GameState world, NPC npc)
    {
        foreach (var trigger in CompletionTriggers)
        {
            switch (trigger.Type)
            {
                case "talk_to_npc":
                    if (npc.Name == trigger.NPCName)
                    {
                        return trigger;
                    }
                    break;
                case "zone_item":
                    if (trigger.ItemId != null && trigger.Room != null)
                    {
                        var room = world.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                        if (room != null && room.IsInside(trigger.ItemId, trigger.Quantity))
                            return trigger;
                    }
                    break;
                case "own_item":
                    if (trigger.ItemId != null)
                    {
                        int count = world.Player.Inventory.Count(i => i.Id == trigger.ItemId);
                        if (count >= trigger.Quantity)
                            return trigger;
                    }
                    break;
            }
        }
        return null;
    }

    public void ExecuteOnFinishActions(GameState world, string decision, TUI tui)
    {
        Item item;
        foreach (OnFinishAction action in OnFinishActions)
        {
            if (decision != "" && action.Decision!= null && action.Decision != decision)
            {
                continue;
            }
            switch (action.Type)
            {
                case "give_item":
                    if (action.ItemId != null)
                    {
                        item = ItemRegistry.CreateItem(action.ItemId);
                        world.Player.Inventory.Add(item);
                    }
                    break;
                case "spawn_item":
                    if (action.ItemId == null || action.ToX == null || action.ToY == null)
                    {
                        continue;
                    }
                    Room? room = world.RoomManager.GetRoom(action.ToX ?? 0, action.ToY ?? 0);
                    if (room != null) {
                        for (int i = 0; i < action.Quantity; i++)
                        {
                            item = ItemRegistry.CreateItem(action.ItemId);
                            room.Items.Add(item);   
                        }
                    }
                    break;
                case "remove_inventory_item":
                    if (action.ItemId != null)
                    {
                        _ = world.Player.RemoveItem(action.ItemId, action.Quantity);
                    }
                    break;
                case "remove_room_item":
                    if (action.ItemId != null)
                    {
                        room = world.RoomManager.GetRoom(action.ToX ?? 0, action.ToY ?? 0);
                        if (room != null)
                        {
                            var items = room.Items.Where(i => i.Id == action.ItemId).ToList();
                            for (int i = 0; i < action.Quantity && items.Count != 0; i++)
                            {
                                room.RemoveItem(items[i]);
                            }
                        }
                    }
                    break;
                case "unlock_quest":
                    if (action.UnlockQuest != null)
                    {
                        if (decision != null)
                        {
                            string questId = action.UnlockQuest[decision];
                            Quest quest = QuestList.Get(questId);
                            world.Player.QuestProgression.AvailableQuests.Add(questId);
                        } else
                        {
                            string questId = action.UnlockQuest[""];
                            Quest quest = QuestList.Get(questId);
                            world.Player.QuestProgression.AvailableQuests.Add(questId);
                        }
                    }
                    break;
                case "final_rating":
                    world.Player.QuestProgression.EvaluateChoices(tui);
                    break;
            }
        }
    }
}

public class CompletionTrigger
{
    public string Type { get; set; } = "talk_to_npc"; // "zone_item", "own_item"
    public string? ItemId { get; set; }        // for inventory or zone items
    public int Quantity { get; set; } = 1;     // minimum number of items required
    public int[]? Room { get; set; }           // [x, y] for "zone_item"
    public string? Decision { get; set; }
    public string? NPCName { get; set; }
    public bool? IsGood {get; set; } = false;
}



public class OnFinishAction
{
    public string? Type { get; set; } // "give_item", "spawn_item", "remove_inventory_item", "remove_room_item", "unlock_quest"
    public int ?ToX { get; set; }
    public int ?ToY { get; set; }
    public string? Npc { get; set; }
    public string? ItemId {get; set; }
    public int Quantity {get; set; } = 1;
    public Dictionary<string, string>? UnlockQuest {get; set;}
    public string? Decision {get; set; } = null;
}