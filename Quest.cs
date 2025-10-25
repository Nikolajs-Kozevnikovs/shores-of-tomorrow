namespace WorldOfZuul 
{
  public class Quest {
    private string Description { get; }
    private readonly List<Quest> Prerequisites;
    private string Objective { get; } // targer progress
    private bool[,] ActiveTiles { get; } // array of coordinates for the tiles where the quest can be completed
    public bool IsCompleted = false;

    public Quest(string description, List<Quest> prerequisites, string objective, bool[,] activeTiles)
    {
        Description = description;
        Prerequisites = prerequisites ?? new List<Quest>();
        Objective = objective;
        ActiveTiles = activeTiles;
    }

    public Quest(string description, Quest prerequisite, string objective, bool[,] activeTiles)
        : this(description, new List<Quest> { prerequisite }, objective, activeTiles) {}

    public Quest(string description, string objective, bool[,] activeTiles)
        : this(description, new List<Quest>(), objective, activeTiles) {}

    public bool ArePrerequisitesCompleted()
    {
      foreach (Quest q in Prerequisites)
      {
        if (!q.IsCompleted)
        {
          return false;
        }
      }

      return true;
    }

    public void RemovePrerequisite(Quest quest)
    {
      Prerequisites.Remove(quest);
    }

    public bool[,] GetActiveTiles()
    {
      return ActiveTiles;
    }
  }
}
