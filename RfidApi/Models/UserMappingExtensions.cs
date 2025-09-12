using RfidApi.Models.Dtos;

namespace RfidApi.Models;

public static class UserMappingExtensions
{
    public static UserDto MapUserToDto(this Users user) =>
        new(user.Email, user.Name ?? string.Empty, user.Role);

    public static Users MapDtoToUser(this UserDto userDto) =>
        new()
        {
            Email = userDto.Email,
            Name = userDto.Name,
            Role = userDto.Role,
        };
}
