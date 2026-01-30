namespace HotelManagementAPI.DTOs;

public class ClientDto
{
    public int ClientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

