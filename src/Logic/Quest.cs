namespace WorldOfZuul.Logic;
public class Quest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string State { get; set; } = "not_started"; // "available", "in_progress", "completed"
    public string GiverNPC { get; set; }
    public string? FinishNPC { get; set; }
    public List<string> VisibilityConditions { get; set; } = [];
    public List<CompletionTrigger> CompletionTriggers { get; set; } = [];
    public List<string> PreQuestDialogue { get; set; } = [];
    public List<string> CompletionDialogue { get; set; } = [];
    public List<OnCompleteAction> OnCompleteActions { get; set; } = [];
    // public List<string> RewardItems { get; set; } = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Quest() {}
    

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Quest(string title, string description, string giverNPC, List<string> visibilityConditions, List<CompletionTrigger> completionTriggers, List<string> preQuestDialogue, List<string> completionDialogue, List<OnCompleteAction> onCompleteActions)
    {
        Title = title;
        Description = description;
        GiverNPC = giverNPC;
        VisibilityConditions = visibilityConditions;
        CompletionTriggers = completionTriggers;
        PreQuestDialogue = preQuestDialogue;
        CompletionDialogue = completionDialogue;
        OnCompleteActions = onCompleteActions;
    }

    public Quest(string title, string description, string giverNPC, string finishNPC, List<string> visibilityConditions, List<CompletionTrigger> completionTriggers, List<string> preQuestDialogue, List<string> completionDialogue, List<OnCompleteAction> onCompleteActions)
    : this(title, description, giverNPC, visibilityConditions, completionTriggers, preQuestDialogue, completionDialogue, onCompleteActions)
    {
        FinishNPC = finishNPC;
    }

    public Quest(string title, string description, string giverNPC, string finishNPC, List<string> visibilityConditions, List<CompletionTrigger> completionTriggers, List<string> preQuestDialogue, List<string> completionDialogue, List<OnCompleteAction> onCompleteActions, string state)
    : this(title, description, giverNPC, visibilityConditions, completionTriggers, preQuestDialogue, completionDialogue, onCompleteActions)
    {
        State = state;
    }
}

public class CompletionTrigger
{
    public string Type { get; set; } = "talk_to_npc"; // "zone_item" or "talk_to_npc"
    public string? Item { get; set; } // for "move_item"
    public int[]? Room { get; set; } // [x, y] for "zone_item"
    public string? Zone { get; set; } // zone ID for "zone_item"
    public string? Npc { get; set; } // for "talk_to_npc"
}

public class OnCompleteAction
{
    public string? Type { get; set; }
    public int ToX { get; set; }
    public int ToY { get; set; }
    public string? Npc { get; set; }
    public string? Item {get; set; }
}