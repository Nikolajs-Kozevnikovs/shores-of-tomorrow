using System;
using System.Collections.Generic;

namespace WorldOfZuul;

// RandomEvents centralises the lightweight random encounter system so it can be reused from the game loop.
public class RandomEvents
{
    // Event types define whether the player just observes something (Passive) or needs to answer (Interactive).
    public enum EventType { Passive, Interactive }

    private readonly List<(string EventId, string Name, EventType Type)> events = new(); // List of registered events, stores event ID, name, and type

    private readonly Random random; // Random number generator for event triggering and selection

    private readonly double triggerChance; // Probability of an event triggering on any given player action (between 0 and 1)

    private static readonly string[] WeatherEvents = // Weather shifts messages (passive event type)
    {
        "A salty breeze picks up, whipping stray leaves across the docks.",
        "Dark clouds gather over the horizon, promising a brief drizzle.",
        "Sunlight pierces the clouds, sparkling on the water like shattered glass.",
        "A flock of gulls spirals overhead, their cries echoing through the bay."
    };

    public RandomEvents(double triggerChance = 0.18) // Constructor and event registration
    {
        if (triggerChance <= 0 || triggerChance >= 1) { // Error exception if triggerChance is out of expected range
            throw new ArgumentOutOfRangeException(nameof(triggerChance), "RandomEvents triggerChance must be between 0 and 1.");
        }
        
        this.triggerChance = triggerChance;

        random = new Random(); // Default RNG

        // Passive events registration
        RegisterEvent("weatherShift", "Sudden Weather Shift", EventType.Passive);

        // Interactive events registration
        RegisterEvent("strangerEncounter", "Stranger Encounter", EventType.Interactive);
    }

    // RegisterEvent lets future developers add more content by specifying an ID, name, and category.
    public void RegisterEvent(string EventId, string name, EventType type)
    {
        if (string.IsNullOrWhiteSpace(EventId))
            throw new ArgumentException("Event Id is required", nameof(EventId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name is required", nameof(name));

        events.Add((EventId, name, type));
    }

    // TryTrigger is called once per player command to decide whether an event should run.
    public void TryTrigger(TUI tui, Room? currentRoom, bool isInDialogue)
    {
        // Never fire events while the player is mid-dialogue with an NPC.
        if (isInDialogue || events.Count == 0)
            return;

        // Roll the probability gate; if it fails, nothing happens this turn.
        if (random.NextDouble() > triggerChance)
            return;

        // Build a list of events that make sense for the current room/location.
        List<(string EventId, string Name, EventType Type)> eligible = new();
        foreach (var evt in events)
        {
            if (CanTrigger(evt.EventId, currentRoom))
                eligible.Add(evt);
        }

        // If nothing applies to this location, quietly exit.
        if (eligible.Count == 0)
            return;

        // Choose one eligible event at random and execute its behaviour.
        var chosen = eligible[random.Next(eligible.Count)];
        ExecuteEvent(chosen.EventId, tui, currentRoom);

        // Refresh the TUI so the newly written text appears immediately.
        tui.DrawCanvas();
    }

    // CanTrigger contains the simple per-event rules that determine when an event is allowed to fire.
    private bool CanTrigger(string EventId, Room? room)
    {
        switch (EventId)
        {
            case "weatherShift":
                // Weather can happen anywhere with a valid room.
                return room != null;
            case "strangerEncounter":
                // The stranger encounter only makes sense on land tiles, so avoid ocean cells.
                return room != null && room.TileIdentifier != Room.Ocean.TileIdentifier;
            default:
                // Unknown IDs default to false so they never trigger until fully defined.
                return false;
        }
    }

    // ExecuteEvent prints the actual narrative for the chosen event and handles any user input.
    private void ExecuteEvent(string EventId, TUI tui, Room? room)
    {
        switch (EventId)
        {
            case "weatherShift":
                // Pick a random weather flavour line and display it.
                int idx = random.Next(WeatherEvents.Length);
                tui.WriteLine("-- Passive Event: Weather --");
                tui.WriteLine(WeatherEvents[idx]);
                break;

            case "strangerEncounter":
                // Present the encounter text and prompt the user for a simple yes/no response.
                tui.WriteLine("-- Interactive Event: Stranger --");
                tui.WriteLine("A passerby notices you and asks if you've been keeping the shoreline clean.");
                tui.WriteLine("How do you respond? (type 'yes' or 'no')");
                tui.DrawCanvas();

                Console.Write("Response: ");
                string? response = Console.ReadLine()?.Trim().ToLowerInvariant();

                // Provide branching reactions based on the player's answer.
                if (response == "yes")
                {
                    tui.WriteLine("The stranger smiles, hands you a reusable water bottle, and thanks you for caring.");
                }
                else if (response == "no")
                {
                    tui.WriteLine("They gently remind you that every small action helps protect the coast.");
                }
                else
                {
                    tui.WriteLine("The stranger tilts their head, unsure of your mumble, and walks away chuckling.");
                }

                break;

            default:
                // Graceful fallback in case someone registers an event but forgets to write its logic.
                tui.WriteLine("The day feels calm. Nothing unexpected happens.");
                break;
        }
    }
}
