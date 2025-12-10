namespace WorldOfZuul.Logic;

using WorldOfZuul.Presentation;

public class RandomEvents
{
    public enum EventType { Passive, Interactive }

    // List of registered events, stores event ID, name, and type
    private readonly List<(string EventId, string Name, EventType Type)> events = new();

    private readonly Random random; // Random number for event triggering probability

    private readonly double triggerChance; // Probability of an event triggering on any given player action (between 0 and 1)

/* Tiles:
    'T', // Townhall
    'S', // Seashore
    'B', // Fishing Boat
    'C', // Coral reef
    'F', // Fish shop
    'R', // Recycling centre
    'F', // Factory
    'O', // Old guy's house
    'L', // Laboratory
    'P', // Food charity point
*/
    private static readonly HashSet<char> WeatherShiftTiles = new()
    {
        'T', // Townhall
        'S', // Seashore
        'B', // Fishing Boat
        'C', // Coral reef
        'O', // Old guy's house
        'P', // Food charity point
    };

    // Tiles where the clean shores encounter should occur (community / shoreline spots).
    private static readonly HashSet<char> CleanShoresTiles = new()
    {
        'T', // Townhall
        'S', // Seashore
        'R', // Recycling centre
        'F', // Factory
        'P', // Food charity point
    };

    public RandomEvents(double triggerChance = 0.25) // Constructor and event registration
    {
        if (triggerChance <= 0 || triggerChance >= 1) { // Error exception if triggerChance is out of expected range
            throw new ArgumentOutOfRangeException(nameof(triggerChance), "RandomEvents triggerChance must be between 0 and 1.");
        }
        
        this.triggerChance = triggerChance;

        random = new Random(); // Default RNG

        // Passive events registration
        RegisterEvent("weatherShift", "Sudden Weather Shift", EventType.Passive);

        // Interactive events registration
        RegisterEvent("cleanShores", "Stranger Encounter", EventType.Interactive);
    }

    // RegisterEvent lets future developers add more content by specifying an ID, name, and category.
    public void RegisterEvent(string EventId, string name, EventType type)
    {
        if(string.IsNullOrWhiteSpace(EventId))
        {
            throw new ArgumentException("Event Id is required", nameof(EventId));
        }
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Event name is required", nameof(name));
        }

        events.Add((EventId, name, type));
    }

    // TryTrigger: check if a random event can be triggered and execute it if so.
    public bool TryTrigger(TUI tui, Room? currentRoom, bool isInDialogue)
    {
        if (isInDialogue || events.Count == 0) // Exit if there is a dialogue active
        {
            return false;
        }

        if (random.NextDouble() >= triggerChance) // Exit if the random roll exceeds the trigger chance
        {
            return false;
        }

        List<(string EventId, string Name, EventType Type)> eligible = new(); // List of eligible events for the current room
        
        foreach(var evt in events)
        {
            if (TriggerCheck(evt.EventId, currentRoom))
                eligible.Add(evt);
        }

        if (eligible.Count == 0) // If no random events are eligible, exit
        {
            return false;
        }

        var chosen = eligible[random.Next(eligible.Count)]; // Pick a random elegible event to be executed
        ExecuteEvent(chosen.EventId, tui, currentRoom); // Execute the chosen random event to be executed

        return true;
    }

    // TriggerCheck: check if it is possible to trigger X event in a given room.
    private static bool TriggerCheck(string EventId, Room? room)
    {
        if (room == null)
        {
            return false;
        }

        char tileId = room.TileIdentifier;

        switch(EventId)
        {
            case "weatherShift": // Can only happen in open spaces.
                return WeatherShiftTiles.Contains(tileId);
            
            case "cleanShores": // Can only happen in community areas.
                return CleanShoresTiles.Contains(tileId);

            default:
                return false;
        }
    }

    // world.RoomManager.GetCurrentRoom().tileIdentifier
    // randomEvents.TryTrigger(tui, world.RoomManager.GetCurrentRoom().tileIdentifier, false);

    
    // ExecuteEvent: execute event by its ID
    private void ExecuteEvent(string EventId, TUI tui, Room? room)
    {
        switch (EventId)
        {
            case "weatherShift":
                // random weather shift line to be sent
                int wsm = random.Next(WeatherEvents.Length);
                tui.WriteLine(" ");
                tui.WriteLine("------- Weather Shift -------");
                tui.WriteLine(WeatherEvents[wsm]);
                tui.WriteLine("------------------------------");
                break;

            case "cleanShores":
                CleanShores(tui);
                break;

            default:
                // default for non existing events called
                tui.WriteLine("------------------------------");
                tui.WriteLine("The day feels calm. Nothing unexpected happens.");
                tui.WriteLine("------------------------------");
                break;
        }
    }

    // Interactive events functions/methods
    private static void CleanShores(TUI tui) // Event Id: cleanShores
    {
        tui.WriteLine(" ");
        tui.WriteLine("----- Stranger Encounter -----");
        tui.WriteLine("A passerby notices you and asks if you've been keeping the sea shores clean.");
        tui.WriteLine("How do you respond? (type 'yes' or 'no')");
        tui.DrawCanvas();

        Console.Write("Response: ");
        string? response = Console.ReadLine()?.Trim().ToLowerInvariant();

        if(response == "yes")
        {
            tui.WriteLine("The stranger smiles, hands you a reusable water bottle, and thanks you for caring.");
            PositiveCleanShoresCounter++;
            CleanShoresCounter++;
        }
        else if(response == "no")
        {
            tui.WriteLine("They gently remind you that every small action helps protect the coast.");
            NegativeCleanShoresCounter++;
            CleanShoresCounter++;
        }
        else
        {
            tui.WriteLine("The stranger tilts their head, unsure of your mumble, and walks away.");
            CleanShoresCounter++;
        }

        // Extra based on accumulated responses
        if(NegativeCleanShoresCounter/3 >= PositiveCleanShoresCounter && CleanShoresCounter >= 10)
        {
            tui.WriteLine("The stranger sighs, disappointed by the lack of care for the environment.");
            tui.WriteLine("\"I hate you!\", they exclaim before walking away.");
            CleanShoresCounter = 1;
            PositiveCleanShoresCounter = 0;
            NegativeCleanShoresCounter = 1;
        }
        else if(CleanShoresCounter >= 5 && PositiveCleanShoresCounter > NegativeCleanShoresCounter)
        {
            tui.WriteLine("Over time, your positive interactions have inspired others to keep the shores clean. Keep it up!");
        }
        else if(CleanShoresCounter >= 5 && NegativeCleanShoresCounter >= PositiveCleanShoresCounter)
        {
            tui.WriteLine("Your repeated negative responses have discouraged the stranger.");
        }
        
        tui.WriteLine("------------------------------");
    }

    // Random events variables
    private static readonly string[] WeatherEvents = // Event Id: weatherShift
    {
        "A salty breeze picks up, whipping stray leaves across the docks.",
        "Dark clouds gather over the horizon, promising a brief drizzle.",
        "Sunlight pierces the clouds, sparkling on the water like shattered glass.",
        "A flock of gulls spirals overhead, their cries echoing through the bay.",
        "The scent of rain fills the air as a gentle shower begins to fall."
    };

    private static int CleanShoresCounter = 0; // Event Id: cleanShores
    private static int PositiveCleanShoresCounter = 0; // Event Id: cleanShores
    private static int NegativeCleanShoresCounter = 0; // Event Id: cleanShores
}