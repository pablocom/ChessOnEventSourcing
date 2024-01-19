using System.Reflection;
using System.Text.Json;
using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Models;

namespace ChessOnEventSourcing.EventStore.Repositories;

public static class EventDeserializer
{
    private static readonly Assembly EventsAssembly = typeof(ChessboardCreated).Assembly;
    
    public static DomainEvent Deserialize(EventDescriptor eventDescriptor)
    {
        var eventType = EventsAssembly.GetType(eventDescriptor.EventType);
        if (eventType is null)
            throw new Exception($"Could not find the corresponding CLR type for '{eventDescriptor.EventType}'");

        var deserializedEvent = JsonSerializer.Deserialize(eventDescriptor.EventData, eventType) as DomainEvent;
        if (deserializedEvent is null)
            throw new Exception($"An error occurred during serialization of event of type {eventType.Name}");

        return deserializedEvent;
    }
}