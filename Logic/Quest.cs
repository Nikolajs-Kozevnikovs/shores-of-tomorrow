public class Quest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string State { get; set; } = "not_started"; // "available", "in_progress", "completed"
    public string GiverNpc { get; set; }
    public string? FinishNpc { get; set; }
    public List<VisibilityCondition> VisibilityConditions { get; set; } = [];
    public List<CompletionTrigger> CompletionTriggers { get; set; } = [];
    public List<string> PreQuestDialogue {get; set; }
    // public List<string> RewardItems { get; set; } = [];
}

public class VisibilityCondition
{
    public List<string> CompletedQuestIds { get; set; } = [];
}

public class CompletionTrigger
{
    public string Type { get; set; } // "zone_item" or "talk_to_npc"
    public string? Item { get; set; } // for "move_item"
    public int[]? Room { get; set; } // [x, y] for "zone_item"
    public string? Zone { get; set; } // zone ID for "zone_item"
    public string? Npc { get; set; } // for "talk_to_npc"
}