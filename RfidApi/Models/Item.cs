using System;
using System.ComponentModel.DataAnnotations;

namespace RfidApi.Models;

public class Item
{
    public int Id { get; set; }

    public required string RfidTag { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "available";
    public string? CertificationCode { get; set; } // Optional
    public required string OwnerName { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
