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
            item.last_updated,
            item.OwnerUser?.Email, // May be null if no owner
            item.last_signal
        );

    public static Item MapDtoToItem(this CreateItemDto itemDto) =>
        new()
        {
            rfid_tag = itemDto.Rfid_tag,
            name = itemDto.Name,
            description = itemDto.Description,
            status = itemDto.Status,
            certification_code = itemDto.Certification_code,
            owner_name = itemDto.Owner_name,
            last_updated = DateTime.UtcNow,
        };

    public static void UpdateItem(this Item item, UpdateItemDto itemDto)
    {
        item.name = itemDto.Name;
        item.description = itemDto.Description ?? item.description;
        item.status = itemDto.Status;
        item.certification_code = itemDto.CertificationCode ?? item.certification_code;
        item.owner_name = itemDto.OwnerName ?? item.owner_name;
        item.last_updated = DateTime.UtcNow;
        item.last_signal = DateTime.UtcNow;
    }
}
