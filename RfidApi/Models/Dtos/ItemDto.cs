using System;

namespace RfidApi.Models.Dtos;

public record ItemDto(
    string Rfid_tag,
    string Name,
    string? Description,
    string Status,
    string? Certification_code,
    string? Owner_name,
    DateTime Last_updated,
    string? Owner_email // Optional
);
