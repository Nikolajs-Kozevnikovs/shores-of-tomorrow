using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;

public class QuestProgression
{
    public List<string> FinishedQuests { get; set; } = [];
    public List<string> AvailableQuests { get; set; } = ["Welcome"];
    public List<string> ActiveQuests { get; set; } = [];
    public Dictionary<string, string> Decisions { get; set; } = []; // <quest: decision>

    public void AcceptQuest(string questId)
    {
        ActiveQuests.Add(questId);
        AvailableQuests.Remove(questId);
    }

    public void FinishQuest(GameState world, string questId, string? decision)
    {
        string? activeQuest = ActiveQuests.Find(i => i == questId);
        if (activeQuest == null)
        {
            throw new Exception("Error: attempted to finish quest when there is no active quest!");
        }
        
        FinishedQuests.Add(activeQuest);

        Quest quest = QuestList.Get(activeQuest);

        
        quest.ExecuteOnFinishActions(world, decision);

        ActiveQuests.Remove(activeQuest);
    }

    public void TryAcceptQuest(NPC npc, TUI tui)
    {
    
        // if npc player is talking to can offer the quest that is available
        foreach (string availableQuest in AvailableQuests)
        {
            Quest q = QuestList.Get(availableQuest);
            if (npc.Name == q.GiverNPC)
            {
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
                    AcceptQuest(availableQuest);
                    tui.WriteLine($"Quest Accepted: {q.Title}");
                    return;
                } else
                {
                    tui.WriteLine("Well, come back when you'll change your mind.");
                    return;
                }
            }
        }
        // available quest is from a different npc: default text
        tui.WriteLine($"{npc.Name}: Hi! How's your day goin'?");
        return;
    }

    public void TryFinishQuest(NPC npc, TUI tui, GameState World)
    {
        if (ActiveQuests.Count == 0)
        {
            tui.WriteLine("Finishing quest failed: no active quest!");
            return;
        }
        foreach (string qId in ActiveQuests) {
            Quest activeQuest = QuestList.Get(qId);
            CompletionTrigger? trigger = activeQuest.FindCompletionTrigger(World, npc);
            if (trigger == null)
            {
                tui.WriteLine("You have not fulfilled the requirements to complete this quest!");
                continue;
            }

            if (trigger.Type == "talk_to_npc")
            {
                if (trigger.NPCName == null || trigger.NPCName != npc.Name)
                {
                    tui.WriteLine($"{npc.Name}: Hi! How's your day goin'?");
                    continue;
                }
            }
            // choose completion dialogue
            List<string> CompletionDialogue;
            if (trigger.Decision == null)
            {
                CompletionDialogue = activeQuest.CompletionDialogue[""];
            } else
            {
                CompletionDialogue = activeQuest.CompletionDialogue[trigger.Decision]; 
            }

            for (int i = 0; i < CompletionDialogue.Count; i++)
            {
                tui.WriteLine(CompletionDialogue[i]);
                Console.ReadKey();
            }

            if (trigger.Decision != null)
            {
                Decisions[activeQuest.Title] = trigger.Decision;
            }

            FinishQuest(World, qId, trigger.Decision);
            return;
        }
    }
}
