using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RfidApi.Models;

[Index(nameof(rfid_tag), IsUnique = true)]
public class Item
{
    public int Id { get; set; }

    public required string rfid_tag { get; set; }
    public required string name { get; set; }
    public string? description { get; set; }
    public string status { get; set; } = "available";
    public string? certification_code { get; set; } // Optional
    public string? owner_name { get; set; }
    public DateTime last_updated { get; set; } = DateTime.UtcNow;
}
