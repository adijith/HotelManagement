using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementAPI.Models;

public class CalendarEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CalendarEntryId { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    public DateTime AssignedDate { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }

    // Navigation property
    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;
}

