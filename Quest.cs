namespace WorldOfZuul 
{
  public class Quest {
    private string description;
    private string questGiverName;
    private int progress; // how much progress the player has made
    private int objective; // targer progress
    private bool isDone; 
    private int[][] activeTiles; // array of coordinates for the tiles that are active

    public Quest(string Description, string QuestGiverName, int Objective, int[][] ActiveTiles) {
      description = Description;
      questGiverName = QuestGiverName;
      objective = Objective;
      progress = 0;
      isDone = false;
      activeTiles = ActiveTiles;
    }

    public bool GetIsDone() {
      return isDone;
    }

    public GetQuestGiver() {
      return questGiver;
    }

    public GetDescription() {
      return description;
    }

    public GetQuestProgression() {
      return {progress, objective};
    }

    public GetActiveTiles() {
      return {activeTiles}
    }

    public IncrementProgress() {
      progress += 1;
      if (progress >= objective) {
         isDone = true;
      }
    }

    
  }
}
