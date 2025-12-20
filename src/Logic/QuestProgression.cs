using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;

public class QuestProgression
{
    public List<string> FinishedQuests { get; set; } = new();
    public string AvailableQuest { get; set; } = "Welcome";
    public string? ActiveQuest { get; set; } = null;
    public Dictionary<string, string> Decisions { get; set; } = new();

    public void AcceptQuest(string questId)
    {
        ActiveQuest = questId;
        AvailableQuest = questId;
    }

    public void FinishQuest(GameState world, string? decision)
    {
        if (ActiveQuest == null)
        {
            throw new Exception("Error: attempted to finish quest when there is no active quest!");
        }

        FinishedQuests.Add(ActiveQuest);

        Quest quest = QuestList.Get(ActiveQuest);
        
        quest.ExecuteOnFinishActions(world, decision);

        ActiveQuest = null;
    }

    public void TryAcceptQuest(NPC npc, TUI tui)
        {
            // if npc player is talking to can offer the quest that is available
            if (npc.QuestsGiven.Any(qName => qName == AvailableQuest))
            {
                Quest q = QuestList.Get(AvailableQuest);
                for (int i = 0; i < q.PreQuestDialogue.Count; i++)
                {
                    tui.WriteLine($"{npc.Name}: {q.PreQuestDialogue[i]}");
                    Console.ReadKey();
                }
                tui.WriteLine("");
                tui.WriteLine("Would you be down to do this?");
                Console.Write("> ");
                string? text = Console.ReadLine();
                if (text != null && text == "yes")
                {
                    AcceptQuest(AvailableQuest);
                    tui.WriteLine($"Quest Accepted: {q.Title}");
                    return;
                } else
                {
                    tui.WriteLine("Well, come back when you'll change your mind.");
                    return;
                }
            }
            // available quest is from a different npc: default text
            tui.WriteLine($"{npc.Name}: Hi! How's your day goin'?");
            return;
        }

        public void TryFinishQuest(NPC npc, TUI tui, GameState World)
        {
            if (ActiveQuest == null)
            {
                tui.WriteLine("Finishing quest failed: no active quest!");
                return;
            }

            Quest activeQuest = QuestList.Get(ActiveQuest);
            CompletionTrigger? trigger = activeQuest.FindCompletionTrigger(World);
            if (trigger == null)
            {
                tui.WriteLine("You have not fulfilled the requirements to complete this quest!");
                return;
            }

            if (trigger.Type == "talk_to_npc")
            {
                if (trigger.NPCName == null || trigger.NPCName != npc.Name)
                {
                    tui.WriteLine($"{npc.Name}: Hi! How's your day goin'?");
                    return;
                }
            }

            for (int i = 0; i < activeQuest.CompletionDialogue.Count; i++)
            {
                tui.WriteLine($"{npc.Name}: {activeQuest.CompletionDialogue[i]}");
                Console.ReadKey();
            }

            if (trigger.Decision != null)
            {
                Decisions[activeQuest.Title] = trigger.Decision;
            }

            FinishQuest(World, trigger.Decision);
        }

    public bool CanFinishQuest(GameState world)
    {
        if (ActiveQuest == null) return false;
        var quest = QuestList.Get(ActiveQuest);
        return quest.IsCompleted(world);
    }
}
