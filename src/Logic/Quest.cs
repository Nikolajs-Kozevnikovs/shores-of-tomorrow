namespace WorldOfZuul.Logic;
internal class Quest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string State { get; set; } = "not_started"; // "available", "in_progress", "completed"
    public string GiverNPC { get; set; }
    public string? FinishNPC { get; set; }
    public List<VisibilityCondition> VisibilityConditions { get; set; } = [];
    public List<CompletionTrigger> CompletionTriggers { get; set; } = [];
    public List<string> PreQuestDialogue {get; set; }
    // public List<string> RewardItems { get; set; } = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Quest() {}
    

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Quest(string title, string description, string giverNPC, List<VisibilityCondition> visibilityConditions, List<CompletionTrigger> completionTriggers, List<string> preQuestDialogue)
    {
        Title = title;
        Description = description;
        GiverNPC = giverNPC;
        VisibilityConditions = visibilityConditions;
        CompletionTriggers = completionTriggers;
        PreQuestDialogue = preQuestDialogue;
    }

    public Quest(string title, string description, string giverNPC, string finishNPC, List<VisibilityCondition> visibilityConditions, List<CompletionTrigger> completionTriggers, List<string> preQuestDialogue)
    : this(title, description, giverNPC, visibilityConditions, completionTriggers, preQuestDialogue)
    {
        FinishNPC = finishNPC;
    }
}

public class VisibilityCondition
{
    public List<string> CompletedQuestIds { get; set; } = [];
}

public class CompletionTrigger
{
    public string Type { get; set; } = "talk_to_npc"; // "zone_item" or "talk_to_npc"
    public string? Item { get; set; } // for "move_item"
    public int[]? Room { get; set; } // [x, y] for "zone_item"
    public string? Zone { get; set; } // zone ID for "zone_item"
    public string? Npc { get; set; } // for "talk_to_npc"
}