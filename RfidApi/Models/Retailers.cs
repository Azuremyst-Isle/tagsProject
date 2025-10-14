using System;

namespace RfidApi.Models;

public class Retailers
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Identifier { get; set; } // unique retailer code (CogiteQ-issued)
    public required string Email { get; set; }
    public string? PublicKey { get; set; } // optional, for RSA verification later
    public bool Accredited { get; set; } = true;
}
