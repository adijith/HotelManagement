using System.ComponentModel.DataAnnotations;

namespace HotelManagementAPI.DTOs;

public class CreateCalendarEntryRequest
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public DateTime AssignedDate { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }
}

