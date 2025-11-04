using System.Reflection.Metadata.Ecma335;

namespace WorldOfZuul 
{
  public enum QuestState { Pending, Completed, Done, Active, Locked };

  public class Quest {
    public string Name { get; private set; }
    public string Description { get; }
    public QuestState State { get; set; }
    // tbd
    public Dictionary<string, Quest> Outcomes { get; private set; }
    public string Objective { get; } // target progress
    public List<int[]> ActiveTiles { get; private set; } // array of coordinates for the tiles where the quest can be completed

    public Quest(string name, string description, string objective, List<int[]> activeTiles, Dictionary<string, Quest> outcomes)
    {
      Name = name;
      Description = description;
      Objective = objective;
      ActiveTiles = activeTiles;
      Outcomes = outcomes ?? [];
    }

    public Quest(string name, string description, string objective, List<int[]> activeTiles)
    {
      Name = name;
      Description = description;
      Objective = objective;
      ActiveTiles = activeTiles;
      Outcomes = [];
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
  }
}
