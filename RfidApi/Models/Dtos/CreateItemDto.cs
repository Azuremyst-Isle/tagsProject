using System;

namespace RfidApi.Models.Dtos;

public record CreateItemDto(
    string RfidTag,
    string Name,
    string? Description,
    string Status,
    string? CertificationCode,
    string? OwnerName
);
