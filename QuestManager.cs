namespace WorldOfZuul;

class QuestManager
{
  private readonly List<Quest> activeQuests = new();
  private readonly List<Quest> availableQuests = new();
  private readonly List<Quest> allQuests = new();
  private readonly bool[,] activeTiles;

  public QuestManager(List<Quest> quests)
  {
    allQuests = quests;
    activeTiles = new bool[10, 10];
  }

  public void AcceptQuest(Quest quest)
  {
    activeQuests.Add(quest);
    availableQuests.Remove(quest);
  }

  public void CompleteQuest(Quest quest)
  {
    quest.IsCompleted = true;
    activeQuests.Remove(quest);
    UpdateAvailableQuests();
  }

  public void UpdateAvailableQuests() 
  {
    foreach (Quest q in allQuests)
    {
      if (!q.IsCompleted && q.ArePrerequisitesCompleted() && !availableQuests.Contains(q))
      {
        availableQuests.Add(q);
      }
    }
  }

  public void UpdateActiveTiles()
  {
    Array.Clear(activeTiles, 0, activeTiles.Length);

    foreach (Quest q in activeQuests)
    {
      bool[,] questTiles = q.GetActiveTiles();
      for (int i = 0; i < activeTiles.GetLength(0); i++)
      {
        for (int j = 0; j < activeTiles.GetLength(1); j++)
        {
          activeTiles[i, j] = activeTiles[i, j] || questTiles[i, j];
        }
      }
    }
  }
  
  public bool[,] GetActiveTiles()
  {
    return activeTiles;
  }
}