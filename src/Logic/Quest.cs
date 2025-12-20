namespace WorldOfZuul.Logic;
public class Quest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> PreQuestDialogue { get; set; } = [];
    public List<string> CompletionDialogue { get; set; } = [];
    public List<CompletionTrigger> CompletionTriggers { get; set; } = [];
    public List<OnFinishAction> OnFinishActions { get; set; } = [];
    public string FinishNPC {get; set; } = "";

    public Quest()
    {
    }

    public bool IsCompleted(GameState world)
    {
        foreach (var trigger in CompletionTriggers)
        {
            switch (trigger.Type)
            {
                case "talk_to_npc":
                    return true;
                case "zone_item":
                    if (trigger.ItemId != null && trigger.Room != null)
                    {
                        var room = world.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                        if (room != null && room.IsInside(trigger.ItemId))
                            return true;
                    }
                    break;
                case "inventory_item":
                    if (trigger.ItemId != null)
                    {
                        int count = world.Player.Inventory.Count(i => i.Id == trigger.ItemId);
                        if (count >= trigger.Quantity)
                            return true;
                    }
                    break;
            }
        }
        return false;
    }
    // this thing doesn't work completely correctly. If you have multiple completion triggers, it's gonna fail
    public CompletionTrigger? FindCompletionTrigger(GameState world)
    {
        foreach (var trigger in CompletionTriggers)
        {
            switch (trigger.Type)
            {
                case "talk_to_npc":
                    return trigger;
                case "zone_item":
                    if (trigger.ItemId != null && trigger.Room != null)
                    {
                        var room = world.RoomManager.GetRoom(trigger.Room[0], trigger.Room[1]);
                        if (room != null && room.IsInside(trigger.ItemId))
                            return trigger;
                    }
                    break;
                case "inventory_item":
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

    public void ExecuteOnFinishActions(GameState world, string? decision)
    {
        Item item;
        foreach (OnFinishAction action in OnFinishActions)
        {
            switch (action.Type)
            {
                case "give_item":
                    if (action.ItemId == null)
                    {
                        continue;
                    }
                    item = ItemRegistry.CreateItem(action.ItemId);
                    world.Player.Inventory.Add(item);
                    break;
                case "spawn_item":
                    if (action.ItemId == null || action.ToX == null || action.ToY == null)
                    {
                        continue;
                    }
                    Room? room = world.RoomManager.GetRoom(action.ToX ?? 0, action.ToY ?? 0);
                    if (room == null) {
                        continue;
                    }
                    item = ItemRegistry.CreateItem(action.ItemId);
                    room.Items.Add(item);
                    break;
                case "remove_inventory_item":
                    if (action.ItemId == null)
                    {
                        continue;
                    }
                    world.Player.Remove(action.ItemId, action.Amount);
                    break;
                case "remove_room_item":
                    if (action.ItemId == null)
                    {
                        continue;
                    }
                    room = world.RoomManager.GetRoom(action.ToX ?? 0, action.ToY ?? 0);
                    if (room == null)
                    {
                        continue;
                    }
                    var items = room.Items.Where(i => i.Id == action.ItemId).ToList();
                    for (int i = 0; i < action.Amount && items.Count != 0; i++)
                    {
                        room.RemoveItem(items[i]);
                    }
                    break;
                case "unlock_quest":
                    // TBD
                    break;
            }
        }
    }
}

public class CompletionTrigger
{
    public string Type { get; set; } = "talk_to_npc"; // "zone_item", "inventory_item", etc.
    public string? ItemId { get; set; }        // for inventory or zone items
    public int Quantity { get; set; } = 1;     // minimum number of items required
    public int[]? Room { get; set; }           // [x, y] for "zone_item"
    public string? Zone { get; set; }          // zone ID for "zone_item"4
    public string? Decision { get; set; }
    public string? NPCName { get; set; }
}



public class OnFinishAction
{
    public string? Type { get; set; } // "give_item", "spawn item", "take_item", "unlock_quest"
    public int ?ToX { get; set; }
    public int ?ToY { get; set; }
    public string? Npc { get; set; }
    public string? ItemId {get; set; }
    public int Amount {get; set; } = 1;
    // unlock quest
}