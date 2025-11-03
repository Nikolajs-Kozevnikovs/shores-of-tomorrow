using System.Reflection.Metadata.Ecma335;

namespace WorldOfZuul 
{
  public enum QuestState { Pending, Completed, Done, Active, Locked }; // no state for yet unavailable quests

  public class Quest {
    public string Name { get; private set; }
    public string Description { get; }
    public string Objective { get; } // for now it is not exactly clear how to use this
    public List<int[]> ActiveTiles { get; private set; } // array of coordinates for the tiles where the quest can be completed
    
    public QuestState State { get; set; }
    
    public Dictionary<string, Quest>? Outcomes;
    public NPC? QuestGiver;

    public Quest(string name, string description, string objective, List<int[]> activeTiles ) // outcomes are cut =, fix later
    {
      Name = name;
      Description = description;
      Objective = objective;
      ActiveTiles = activeTiles;
    }

    public void Accept()
    {
      State = QuestState.Active;
    }

    // complete the Quest, unlock the quests that depend on the outcome and lock the ones that don't
    public void Complete(string outcome)
    {
      State = QuestState.Completed;

      if (Outcomes[outcome].State != QuestState.Locked)
      {
        Outcomes[outcome].State = QuestState.Pending;
      }

      foreach (var o in Outcomes)
      {
        if (o.Key == outcome)
        {
          continue;
        }


        o.Value.State = QuestState.Locked;
      }
    }

    public void setQuestGiver(NPC newNPC) {
      questGiver = newNPC;
    }

    public void setOutcomes(Dictionary<string, Quest> newOutcomes) {
      outcomes = newOutcomes;
    }
  }
}
