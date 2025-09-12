using RfidApi.Models.Dtos;

namespace RfidApi.Models;

public static class UserMappingExtensions
{
    public static UserDto MapUserToDto(this Users user) =>
        new(user.Email, user.Name ?? string.Empty, user.Role);
}
