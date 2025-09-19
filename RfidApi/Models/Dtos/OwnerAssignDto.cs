using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RfidApi.Models.Dtos;

public record OwnerAssignDto(
    [property: JsonPropertyName("owner_email")]
    [param: Required(AllowEmptyStrings = false, ErrorMessage = "owner_email is required")]
    [param: EmailAddress(ErrorMessage = "owner_email must be a valid email")]
        string OwnerEmail
);
