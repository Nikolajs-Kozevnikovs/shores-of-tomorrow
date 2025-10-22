namespace WorldOfZuul
{
    public class Item
    {
        private string name;
        private string description;
        private float weight; // Just in case we need it
        private float value;

        public Item(string Name, string Description, float Weight, float Value)
        {
            name = Name;
            description = Description;
            weight = Weight;
            value = Value;
        }

        public string GetName()
        {
            return name;
        }

        public string GetDescription()
        {
            return description;
        }

        public float GetWeight()
        {
            return weight;
        }

        public float GetValue()
        {
            return value;
        }

        public static bool DoesItemExist(Item item)
        {
            if (item != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}