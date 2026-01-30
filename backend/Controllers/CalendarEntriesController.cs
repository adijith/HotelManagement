using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementAPI.Data;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.Models;

namespace HotelManagementAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarEntriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CalendarEntriesController> _logger;

    public CalendarEntriesController(ApplicationDbContext context, ILogger<CalendarEntriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/calendarentries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarEntryDto>>> GetCalendarEntries()
    {
        var entries = await _context.CalendarEntries
            .Include(e => e.Client)
            .Select(e => new CalendarEntryDto
            {
                CalendarEntryId = e.CalendarEntryId,
                ClientId = e.ClientId,
                ClientName = e.Client.FullName,
                AssignedDate = e.AssignedDate,
                Notes = e.Notes
            })
            .OrderBy(e => e.AssignedDate)
            .ToListAsync();

        return Ok(entries);
    }

    // GET: api/calendarentries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CalendarEntryDto>> GetCalendarEntry(int id)
    {
        var entry = await _context.CalendarEntries
            .Include(e => e.Client)
            .FirstOrDefaultAsync(e => e.CalendarEntryId == id);

        if (entry == null)
        {
            _logger.LogWarning("Calendar entry with ID {EntryId} not found", id);
            return NotFound(new { message = $"Calendar entry with ID {id} not found" });
        }

        var entryDto = new CalendarEntryDto
        {
            CalendarEntryId = entry.CalendarEntryId,
            ClientId = entry.ClientId,
            ClientName = entry.Client.FullName,
            AssignedDate = entry.AssignedDate,
            Notes = entry.Notes
        };

        return Ok(entryDto);
    }

    // GET: api/calendarentries/client/5
    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<CalendarEntryDto>>> GetCalendarEntriesByClient(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
        {
            return NotFound(new { message = $"Client with ID {clientId} not found" });
        }

        var entries = await _context.CalendarEntries
            .Include(e => e.Client)
            .Where(e => e.ClientId == clientId)
            .Select(e => new CalendarEntryDto
            {
                CalendarEntryId = e.CalendarEntryId,
                ClientId = e.ClientId,
                ClientName = e.Client.FullName,
                AssignedDate = e.AssignedDate,
                Notes = e.Notes
            })
            .OrderBy(e => e.AssignedDate)
            .ToListAsync();

        return Ok(entries);
    }

    // GET: api/calendarentries/date/2026-02-01
    [HttpGet("date/{date}")]
    public async Task<ActionResult<IEnumerable<CalendarEntryDto>>> GetCalendarEntriesByDate(DateTime date)
    {
        var entries = await _context.CalendarEntries
            .Include(e => e.Client)
            .Where(e => e.AssignedDate.Date == date.Date)
            .Select(e => new CalendarEntryDto
            {
                CalendarEntryId = e.CalendarEntryId,
                ClientId = e.ClientId,
                ClientName = e.Client.FullName,
                AssignedDate = e.AssignedDate,
                Notes = e.Notes
            })
            .OrderBy(e => e.AssignedDate)
            .ToListAsync();

        return Ok(entries);
    }

    // GET: api/calendarentries/range?startDate=2026-02-01&endDate=2026-02-28
    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<CalendarEntryDto>>> GetCalendarEntriesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var entries = await _context.CalendarEntries
            .Include(e => e.Client)
            .Where(e => e.AssignedDate.Date >= startDate.Date && e.AssignedDate.Date <= endDate.Date)
            .Select(e => new CalendarEntryDto
            {
                CalendarEntryId = e.CalendarEntryId,
                ClientId = e.ClientId,
                ClientName = e.Client.FullName,
                AssignedDate = e.AssignedDate,
                Notes = e.Notes
            })
            .OrderBy(e => e.AssignedDate)
            .ToListAsync();

        return Ok(entries);
    }

    // POST: api/calendarentries
    [HttpPost]
    public async Task<ActionResult<CalendarEntryDto>> CreateCalendarEntry([FromBody] CreateCalendarEntryRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if client exists
        var client = await _context.Clients.FindAsync(request.ClientId);
        if (client == null)
        {
            return BadRequest(new { message = $"Client with ID {request.ClientId} not found" });
        }

        var entry = new CalendarEntry
        {
            ClientId = request.ClientId,
            AssignedDate = request.AssignedDate,
            Notes = request.Notes
        };

        _context.CalendarEntries.Add(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Calendar entry created with ID {EntryId} for Client {ClientId}", entry.CalendarEntryId, entry.ClientId);

        // Reload with client info
        await _context.Entry(entry).Reference(e => e.Client).LoadAsync();

        var entryDto = new CalendarEntryDto
        {
            CalendarEntryId = entry.CalendarEntryId,
            ClientId = entry.ClientId,
            ClientName = entry.Client.FullName,
            AssignedDate = entry.AssignedDate,
            Notes = entry.Notes
        };

        return CreatedAtAction(nameof(GetCalendarEntry), new { id = entry.CalendarEntryId }, entryDto);
    }

    // PUT: api/calendarentries/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCalendarEntry(int id, [FromBody] UpdateCalendarEntryRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entry = await _context.CalendarEntries.FindAsync(id);

        if (entry == null)
        {
            _logger.LogWarning("Calendar entry with ID {EntryId} not found for update", id);
            return NotFound(new { message = $"Calendar entry with ID {id} not found" });
        }

        // Check if client exists
        var client = await _context.Clients.FindAsync(request.ClientId);
        if (client == null)
        {
            return BadRequest(new { message = $"Client with ID {request.ClientId} not found" });
        }

        // Update entry properties
        entry.ClientId = request.ClientId;
        entry.AssignedDate = request.AssignedDate;
        entry.Notes = request.Notes;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Calendar entry with ID {EntryId} updated successfully", id);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CalendarEntryExists(id))
            {
                return NotFound(new { message = $"Calendar entry with ID {id} not found" });
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/calendarentries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCalendarEntry(int id)
    {
        var entry = await _context.CalendarEntries.FindAsync(id);

        if (entry == null)
        {
            _logger.LogWarning("Calendar entry with ID {EntryId} not found for deletion", id);
            return NotFound(new { message = $"Calendar entry with ID {id} not found" });
        }

        _context.CalendarEntries.Remove(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Calendar entry with ID {EntryId} deleted successfully", id);

        return NoContent();
    }

    private async Task<bool> CalendarEntryExists(int id)
    {
        return await _context.CalendarEntries.AnyAsync(e => e.CalendarEntryId == id);
    }
}

