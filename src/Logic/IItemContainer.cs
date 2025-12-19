namespace WorldOfZuul.Logic;

public interface IItemContainer
{
    List<Item> Items { get; }

    void AddItem(Item item)
    {
        Items.Add(item);
    }

    bool RemoveItem(Item item)
    {
        return Items.Remove(item);
    }

    bool IsInside(string itemId)
    {
        return Items.Any(i => i.Id == itemId);
    }
}
