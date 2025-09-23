using System;
using System.Text.Json.Serialization;

namespace RfidApi.Models.Dtos;

public record UpdateItemDto(
    string Name,
    string? Description,
    string Status,
    [property: JsonPropertyName("certification_code")] string? CertificationCode,
    [property: JsonPropertyName("owner_name")] string? OwnerName
);
// Note: All fields except Description, CertificationCode, and OwnerName are required
// because they are essential for the item's identity and status.
