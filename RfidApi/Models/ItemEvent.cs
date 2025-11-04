using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RfidApi.Models;

public class ItemEvent
{
    public int Id { get; set; } // PK
    public string RfidTag { get; set; } = default!;
    public int ItemId { get; set; } // FK -> Item.Id

    public required string EventType { get; set; } // created, updated, deleted, ownership_assigned
    public string? EventPayload { get; set; } // JSON string with details
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ActorEmail { get; set; } = null; // Email of the user who performed the action, if available
}

public static class EventTypes
{
    public const string Created = "created";
    public const string Updated = "updated";
    public const string Deleted = "deleted";
    public const string OwnershipAssigned = "ownership_assigned";
    public const string Signal = "signal";
}
