// class Program
// {
//     static void Main() {
//         NPC person = new NPC("Diego", "fisherman", "boy", "quest");
//         Console.WriteLine(person.getName());
//         Console.WriteLine(person);
//         person.Talk();
//     }
// }
namespace WorldOfZuul 
{
    public class NPC {
        private string name;
        private string role;
        private string description;
        // private List<Quest> quests;

        public NPC(string nameValue, string roleValue, string descriptionValue) {
            name = nameValue;
            role = roleValue; // I am not sure if we need this
            description = descriptionValue;
            // quests = questsValue;
        }

        public string GetName() {
            return name;
        }

        public string GetRole() {
            return role;
        }

        public string GetDescription() {
            return description;
        }

        // public string GetQuest() {
        //     return quest;
        // }
        // we'll need to add rendering to that
        // public override string ToString() {
        //     // return $"Name: {name}, Role: {role}, Description: {description}, Quest: {quest}";
        // }
        // tbd, commandParser in not involved
        public void Talk() {
            Console.WriteLine($"{name} says: Hello!");
            Console.ReadLine();
            Console.WriteLine("Do you want to complete a quest?");
            Console.Write("Yes/No: ");
            string ?reply = Console.ReadLine();
            if (reply == "Yes") {
                Console.WriteLine("Do this and this.");
            } else if (reply == "No") {
                Console.WriteLine("Its important!");
            } else {
                Console.WriteLine("Incorrect input.");
            }

        }
    }
}