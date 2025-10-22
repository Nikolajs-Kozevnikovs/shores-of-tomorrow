namespace WorldOfZuul  {
    public class Inventory
    {
        private List<Item> itemsInInventory; // The inventory itself.

        public Inventory()
        {
            itemsInInventory = new List<Item>(); // Constructor for the inventory list.
        }

        public void AddItem(Item item)
        {
            itemsInInventory.Add(item); // Adds an item to the inventory.
        }

        public void RemoveItem(Item item)
        {
            itemsInInventory.Remove(item); // Removes an item from the inventory.
        }

        public List<Item> GetItems()
        {
            return itemsInInventory; // Returns the list of items in the inventory.
        }

        public bool IsItemInInventory(Item item)
        {
            return itemsInInventory.Contains(item); // Checks if an item is in the inventory.
        }

        public string ItemInfo(Item item)
        {
            if (IsItemInInventory(item))
            {
                return $"Name: {item.GetName()}\nDescription: {item.GetDescription()}\nWeight: {item.GetWeight()}\nValue: {item.GetValue()}";
            }
            else
            {
                return "You do not have " + item.GetName() + " in your inventory.";
            }
        }
    }
}