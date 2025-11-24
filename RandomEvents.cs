using System;
using System.Collections.Generic;

namespace WorldOfZuul;

// RandomEvents centralises the lightweight random encounter system so it can be reused from the game loop.
public class RandomEvents
{
    // Event types define whether the player just observes something (Passive) or needs to answer (Interactive).
    public enum EventType { Passive, Interactive }

    private readonly List<(string EventId, string Name, EventType Type)> events = new(); // List of registered events, stores event ID, name, and type

    private readonly Random random; // Random number for event triggering probability

    private readonly double triggerChance; // Probability of an event triggering on any given player action (between 0 and 1)

    public RandomEvents(double triggerChance = 0.99) // Constructor and event registration
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
    public void TryTrigger(TUI tui, Room? currentRoom, bool isInDialogue)
    {
        if (isInDialogue || events.Count == 0) // Exit if there is a dialogue active
        {
            return;
        }

        if (random.NextDouble() > triggerChance) // Exit if the random roll exceeds the trigger chance
        {
            return;
        }

        List<(string EventId, string Name, EventType Type)> eligible = new(); // List of eligible events for the current room
        
        foreach(var evt in events)
        {
            if (TriggerCheck(evt.EventId, currentRoom))
                eligible.Add(evt);
        }

        if (eligible.Count == 0) // If no random events are eligible, exit
            return;

        var chosen = eligible[random.Next(eligible.Count)]; // Pick a random elegible event to be executed
        ExecuteEvent(chosen.EventId, tui, currentRoom); // Execute the chosen random event to be executed

        tui.DrawCanvas();
    }

    // TriggerCheck: check if it is possible to trigger X event in a given room.
    private static bool TriggerCheck(string EventId, Room? room)
    {
        if (room == null)
        {
            return false;
        }

        string id = room.TileIdentifier;

        switch(EventId)
        {
            case "weatherShift": // Can only happen in open spaces.
                return id == Room.SeaShore.TileIdentifier
                    || id == Room.Ocean.TileIdentifier
                    || id == Room.CoralReef.TileIdentifier
                    || id == Room.Blank.TileIdentifier
                    || id == Room.Lake.TileIdentifier
                    || id == Room.Lake2.TileIdentifier
                    || id == Room.Boat.TileIdentifier;
            
            case "cleanShores": // Can only happen in community areas.
                return id == Room.SeaShore.TileIdentifier
                    || id == Room.Blank.TileIdentifier
                    || id == Room.Lake.TileIdentifier
                    || id == Room.Lake2.TileIdentifier
                    || id == Room.Townhall.TileIdentifier;

            default:
                return false;
        }
    }

    // ExecuteEvent: execute event by its ID
    private void ExecuteEvent(string EventId, TUI tui, Room? room)
    {
        switch (EventId)
        {
            case "weatherShift":
                // Pick a random weather shift line and display it.
                int wsm = random.Next(WeatherEvents.Length);
                tui.WriteLine("-- Random Passive Event: Weather Shift --");
                tui.WriteLine(WeatherEvents[wsm]);
                tui.DrawCanvas();
                break;

            case "cleanShores":
                CleanShores(tui);
                tui.DrawCanvas();
                break;

            default:
                // Graceful fallback in case someone registers an event but forgets to write its logic.
                tui.WriteLine("The day feels calm. Nothing unexpected happens.");
                tui.DrawCanvas();
                break;
        }
    }

    // Interactive events functions/methods
    private static void CleanShores(TUI tui) // Event Id: cleanShores
    {
        tui.WriteLine("-- Random Interactive Event: Stranger --");
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
            tui.WriteLine("Your repeated negative responses have discouraged the stranger.");
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
            tui.WriteLine("Despite your efforts, it seems some people remain indifferent. The shores still need more care.");
        }
        
    }

    // Random events variables
    private static readonly string[] WeatherEvents = // Event Id: weatherShift
    {
        "A salty breeze picks up, whipping stray leaves across the docks.",
        "Dark clouds gather over the horizon, promising a brief drizzle.",
        "Sunlight pierces the clouds, sparkling on the water like shattered glass.",
        "A flock of gulls spirals overhead, their cries echoing through the bay."
    };

    private static int CleanShoresCounter = 0; // Event Id: cleanShores
    private static int PositiveCleanShoresCounter = 0; // Event Id: cleanShores
    private static int NegativeCleanShoresCounter = 0; // Event Id: cleanShores
}