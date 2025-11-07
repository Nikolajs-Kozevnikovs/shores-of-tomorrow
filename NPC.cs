namespace WorldOfZuul 
{
    public class NPC {
        private string name;
        private string role;
        private string description;
        public List<string> dialogues;
        public Quest quest;
        
        

        public NPC(string nameValue, string roleValue, string descriptionValue, List<string> dialogueValue, Quest questValue) {
            name = nameValue;
            role = roleValue; // I am not sure if we need this
            description = descriptionValue;
            dialogues = dialogueValue;
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
        
         public void Talk(TUI tui)
        {
            for (int i = 0; i < dialogues.Count; i++) {
                tui.WriteLine($"{GetName()}: {dialogues[i]}");
                tui.DrawCanvas();
                Console.ReadLine();
            }

            tui.WriteLine("Would you help me with " + quest.Name + "? (\"yes\" / \"no\")");
            tui.DrawCanvas();
            string? input = Console.ReadLine()?.ToLower();

            if (input == "yes")
            {
                //Quest.StartQuest();
                tui.WriteLine("Nikolas: Thanks! "); // can add something
            }
            else
            {
                tui.WriteLine("Maybe next time");
            }
        }
    }
}
