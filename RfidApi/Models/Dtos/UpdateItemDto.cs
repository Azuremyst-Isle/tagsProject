using System;

namespace RfidApi.Models.Dtos;

public record UpdateItemDto(
    string Name,
    string? Description,
    string Status,
    string? CertificationCode,
    string? OwnerName
);
