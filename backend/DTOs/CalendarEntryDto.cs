namespace HotelManagementAPI.DTOs;

public class CalendarEntryDto
{
    public int CalendarEntryId { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public string? Notes { get; set; }
}

