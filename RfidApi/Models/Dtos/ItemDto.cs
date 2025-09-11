using System;

namespace RfidApi.Models.Dtos;

public record ItemDto(
    string RfidTag,
    string Name,
    string? Description,
    string Status,
    string? CertificationCode,
    string? OwnerName,
    DateTime LastUpdated
);
