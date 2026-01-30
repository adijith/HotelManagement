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
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(ApplicationDbContext context, ILogger<ClientsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/clients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        var clients = await _context.Clients
            .Select(c => new ClientDto
            {
                ClientId = c.ClientId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(clients);
    }

    // GET: api/clients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            _logger.LogWarning("Client with ID {ClientId} not found", id);
            return NotFound(new { message = $"Client with ID {id} not found" });
        }

        var clientDto = new ClientDto
        {
            ClientId = client.ClientId,
            FullName = client.FullName,
            Email = client.Email,
            Phone = client.Phone,
            CreatedAt = client.CreatedAt
        };

        return Ok(clientDto);
    }

    // POST: api/clients
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if email already exists
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == request.Email);
            
            if (existingClient != null)
            {
                return BadRequest(new { message = "A client with this email already exists" });
            }
        }

        var client = new Client
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.Now
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Client created with ID {ClientId}", client.ClientId);

        var clientDto = new ClientDto
        {
            ClientId = client.ClientId,
            FullName = client.FullName,
            Email = client.Email,
            Phone = client.Phone,
            CreatedAt = client.CreatedAt
        };

        return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, clientDto);
    }

    // PUT: api/clients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            _logger.LogWarning("Client with ID {ClientId} not found for update", id);
            return NotFound(new { message = $"Client with ID {id} not found" });
        }

        // Check if email already exists for another client
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == request.Email && c.ClientId != id);
            
            if (existingClient != null)
            {
                return BadRequest(new { message = "A client with this email already exists" });
            }
        }

        // Update client properties
        client.FullName = request.FullName;
        client.Email = request.Email;
        client.Phone = request.Phone;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Client with ID {ClientId} updated successfully", id);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ClientExists(id))
            {
                return NotFound(new { message = $"Client with ID {id} not found" });
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/clients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            _logger.LogWarning("Client with ID {ClientId} not found for deletion", id);
            return NotFound(new { message = $"Client with ID {id} not found" });
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Client with ID {ClientId} deleted successfully", id);

        return NoContent();
    }

    private async Task<bool> ClientExists(int id)
    {
        return await _context.Clients.AnyAsync(e => e.ClientId == id);
    }
}

