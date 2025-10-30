namespace WorldOfZuul;
// you can't have multiple quests as prerequisites
class QuestManager
{

  public readonly List<Quest> questList = new();
  private readonly bool[,] activeTiles;

  public QuestManager(List<Quest> quests)
  {
    questList = quests;
    activeTiles = new bool[10, 10];
  }
  // clumsy but it works
  public void AcceptQuest(string questName)
  {
    var q = questList.Find(x => x.Name == questName);
    q?.Accept();
  }
  // clumsy but it works
  public void CompleteQuest(string questName, string outcome) {
    var q = questList.Find(x => x.Name == questName);
    q?.Complete(outcome);
  }

  public void UpdateActiveTiles()
  {
    Array.Clear(activeTiles, 0, activeTiles.Length);

    foreach (Quest q in questList)
    {
      if (q.State != QuestState.Active)
      {
        continue;
      }

      foreach (int[] coords in q.ActiveTiles)
      {
        activeTiles[coords[0], coords[1]] = true;
      }
    }
  }
  
  public bool[,] GetActiveTiles()
  {
    return activeTiles;
  }
}