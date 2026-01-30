using Microsoft.AspNetCore.Mvc;
using HotelManagementAPI.DTOs;
using HotelManagementAPI.Services;

namespace HotelManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterAsync(request);

        if (response == null)
        {
            _logger.LogWarning("Failed registration attempt for username: {Username}", request.Username);
            return BadRequest(new { message = "Username or email already exists" });
        }

        _logger.LogInformation("User {Username} registered successfully", request.Username);
        return Ok(response);
    }
}

