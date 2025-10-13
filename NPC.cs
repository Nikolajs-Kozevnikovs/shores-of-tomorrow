class Program
{
    static void Main() {
        NPC person = new NPC("Diego", "fisherman", "boy", "quest");
        Console.WriteLine(person.getName());
        Console.WriteLine(person);
        person.Talk();
    }
}

public class NPC {
    private string name;
    private string role;
    private string description;
    private string quest;

    public NPC (string nameValue, string roleValue, string descriptionValue, string questValue) {
        name = nameValue;
        role = roleValue;
        description = descriptionValue;
        quest = questValue;
    }

    public string getName() {
        return name;
    }

    public string getRole() {
        return role;
    }

    public string getDescription() {
        return description;
    }

    public string getQuest() {
        return quest;
    }   

    public override string ToString() {
    return $"Name: {name}, Role: {role}, Description: {description}, Quest: {quest}";
    }  

    public void Talk() {
        Console.WriteLine($"{name} says: Hello!");
        Console.ReadLine();
        Console.WriteLine("Do you want to complete a quest?");
        Console.Write("Yes/No: ");
        string reply = Console.ReadLine();
        if (reply == "Yes") {
            Console.WriteLine("Do this and this.");
        } else if (reply == "No") {
            Console.WriteLine("Its important!");
        } else {
            Console.WriteLine("Incorrect input.");
        }

    } 
}