namespace WorldOfZuul 
{
  public class Quest {
    private string description;
    private string questGiverName;
    private int progress; // how much progress the player has made
    private int objective; // targer progress
    private bool done;
    private int[][] activeTiles; // array of coordinates for the tiles that are active

    public Quest(string Description, string QuestGiverName, int Objective, int[][] ActiveTiles) {
      description = Description;
      questGiverName = QuestGiverName;
      objective = Objective;
      progress = 0;
      done = false;
      activeTiles = ActiveTiles;
    }

    public bool IsDone() {
      return done;
    }

    public string GetQuestGiver() {
      return questGiverName;
    }

    public string GetDescription() {
      return description;
    }

    // public int[] GetQuestProgression() {
    //   return {progress, objective};
    // }

    // public GetActiveTiles() {
    //   return {activeTiles}
    // }

    public void IncrementProgress() {
      progress += 1;
      if (progress >= objective) {
         done = true;
      }
    }

    
  }
}
