// using WorldOfZuul.Logic;

// namespace UnitTests;

// public class QuestManagerTests
// {
//     [Test]
//     public void CheckCompletion()
//     {
//         GameState world = new GameState(3, 3);

//         world.QuestManager.Quests.Clear();

//         Quest testQuest = new Quest();
//         testQuest.Title = "Test Quest";
//         testQuest.Description = "Talk to the old man";
//         testQuest.State = "in_progress";
//         testQuest.GiverNPC = "old_man";
//         testQuest.CompletionTriggers = new List<CompletionTrigger>
//         {
//             new CompletionTrigger
//             {
//                 Type = "talk_to_npc",
//                 Npc = "old_man"
//             }
//         };
        
//         world.QuestManager.Quests.Add("test_quest", testQuest);
//         world.QuestManager.CheckCompletion("test_quest", "old_man");

//         Assert.That(testQuest.State, Is.EqualTo("completed"), "Quest should be marked as completed after talking to NPC");
//     }
// }