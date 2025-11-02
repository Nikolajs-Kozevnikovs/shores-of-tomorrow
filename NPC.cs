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
        public List<string> dialogue;
        public Quest quest;
        
        

        public NPC(string nameValue, string roleValue, string descriptionValue, List<string> dialogueValue, Quest questValue) {
            name = nameValue;
            role = roleValue; // I am not sure if we need this
            description = descriptionValue;
            dialogue = dialogueValue;
            quest = questValue;
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
         public void Talk()
        {
            for (int i = 0; i < dialogue.Count; i++) {
                Console.WriteLine($"{GetName()}: {dialogue[i]}");
                Console.ReadLine();
            }
            
            Console.WriteLine("Would you help me with " + quest.Name + "?");
            string? input = Console.ReadLine()?.ToLower();

            if (input == "yes")
            {
                //Quest.StartQuest();
                Console.WriteLine("Thanks! "); // can add something
            }
            else
            {
                Console.WriteLine("Maybe next time");
            }
            }
        }
    }
