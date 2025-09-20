using System;

namespace RfidApi.Models.Dtos;

public record EventsDto
{
    public string EventType { get; init; }
    public string EventPayload { get; init; }
    public DateTime CreatedAt { get; init; }
}
