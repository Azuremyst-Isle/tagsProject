using System;
using System.Text.Json.Serialization;

namespace RfidApi.Models.Dtos;

public record EventsDto
{
    [property: JsonPropertyName("event_type")]
    public required string EventType { get; init; }

    [property: JsonPropertyName("event_payload")]
    public string? EventPayload { get; init; }

    [property: JsonPropertyName("created_at")]
    public required DateTime CreatedAt { get; init; }
}
