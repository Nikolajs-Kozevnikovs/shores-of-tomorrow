namespace WorldOfZuul.Logic;

public static class ItemRegistry
{
    private static readonly Dictionary<string, string> _items = new();

    public static void Register(string id, string name)
    {
        _items[id] = name;
    }

    public static Item CreateItem(string id)
    {
        return new Item { Id = id, Name = _items[id] };
    }

    public static Dictionary<string, string> GetItems()
    {
        return _items;
    }
}
