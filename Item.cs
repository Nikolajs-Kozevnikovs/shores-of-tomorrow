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

        public string getName()
        {
            return name;
        }

        public string getDescription()
        {
            return description;
        }

        public float getWeight()
        {
            return weight;
        }

        public float getValue()
        {
            return value;
        }
    }
}