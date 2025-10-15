using System;

namespace RfidApi.Models.Dtos;

public record CreateItemDto(
    string Rfid_tag,
    string Name,
    string? Description,
    string Status,
    string? Certification_code,
    string? Owner_name,
    int Retailer_id
);
