using System;
using Microsoft.EntityFrameworkCore;

namespace RfidApi.Models;

[Index(nameof(Email), IsUnique = true)]
public class Users
{
    public int Id { get; set; } // PK
    public required string Email { get; set; } // unique
    public string? Name { get; set; }
    public string Role { get; set; } = "user"; // future use: admin, staff, etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
