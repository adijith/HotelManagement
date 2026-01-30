using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs;

public class CreateClientRequest
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }
}

