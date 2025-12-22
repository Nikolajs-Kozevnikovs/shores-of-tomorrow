using WorldOfZuul.Presentation;

namespace WorldOfZuul.Logic;

public class QuestProgression
{
    public List<string> FinishedQuests { get; set; } = [];
    public List<string> AvailableQuests { get; set; } = ["Welcome"];
    public List<string> ActiveQuests { get; set; } = [];
    public int GoodDesicisionCount {get; set; } = 0;

    public void AcceptQuest(string questId)
    {
        ActiveQuests.Add(questId);
        AvailableQuests.Remove(questId);
    }

    public void FinishQuest(GameState world, string questId, string? decision, TUI tui)
    {
        string? activeQuest = ActiveQuests.Find(i => i == questId);
        if (activeQuest == null)
        {
            throw new Exception("Error: attempted to finish quest when there is no active quest!");
        }
        
        FinishedQuests.Add(activeQuest);

        Quest quest = QuestList.Get(activeQuest);
        decision ??= "";
        quest.ExecuteOnFinishActions(world, decision, tui);

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

            if (trigger.IsGood != null && trigger.IsGood == true)
            {
                GoodDesicisionCount++;
            }

            FinishQuest(World, qId, trigger.Decision, tui); // temp tui
            return;
        }
    }

    public void EvaluateChoices(TUI tui)
    {
        switch (GoodDesicisionCount)
        {
            case 0:
            case 1: 
                tui.WriteLine("Hey again! I see you've done a lot of work. We're ready to announce the results! You mostly chose unsustainable paths that led to the island's collapse. You may have become rich, but the island is now uninhabitable. We don't believe you're the one we were searching for. Better luck next time...");
                break;
            case 2: 
                tui.WriteLine("Hey again! I see you've put in a lot of effort. Thank you for that, even though not all your choices had good results. We're ready to announce the results! You mostly took unsustainable paths, which hurt the environment and the villagers. We don't believe you're the right person for this role. Better luck next time...");
                break;
            case 3: 
                tui.WriteLine("Hey again! I see you've done a lot of work. Thank you so much - we all truly appreciate it. We're ready to announce the results! You made choices that brought both good and bad for the environment, the villagers, and yourself. That's what we call balance. We believe you will only get better. Congratulations - you're our new manager!");
                break;
            case 4: 
                tui.WriteLine("Hey again! I see you've done a lot of great work. Thank you so much - we all really appreciate it. We're ready to announce the results! You've done almost everything right for the environment and cared for the villagers. The island is thriving! We believe you will only get better. Congratulations - you're our new manager!");
                break;
            case 5: 
            case 6: 
            default:
                tui.WriteLine("Hey again! I see you've done a lot of great work. Thank you so much - we all really appreciate it. We're ready to share the results! You took good care of the environment and helped the villagers. You might not have much money now, but the island is thriving because of you. Congratulations - you're our new manager!");
                break;
        }
    }
}
