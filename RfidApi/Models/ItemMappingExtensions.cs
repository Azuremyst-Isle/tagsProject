using RfidApi.Models.Dtos;

namespace RfidApi.Models;

public static class ItemMappingExtensions
{
    public static ItemDto MapItemToDto(this Item item) =>
        new(
            item.rfid_tag,
            item.name,
            item.description,
            item.status,
            item.certification_code,
            item.owner_name,
            item.last_updated
        );

    public static Item MapDtoToItem(this CreateItemDto itemDto) =>
        new()
        {
            rfid_tag = itemDto.RfidTag,
            name = itemDto.Name,
            description = itemDto.Description,
            status = itemDto.Status,
            certification_code = itemDto.CertificationCode,
            owner_name = itemDto.OwnerName,
            last_updated = DateTime.UtcNow,
        };
}
