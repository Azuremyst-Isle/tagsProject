using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RfidApi.Models;

public class ItemEvent
{
    public int Id { get; set; } // PK
    public int ItemId { get; set; } // FK -> Item.Id

    public required string EventType { get; set; } // created, updated, deleted, ownership_assigned
    public string? EventPayload { get; set; } // JSON string with details
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
