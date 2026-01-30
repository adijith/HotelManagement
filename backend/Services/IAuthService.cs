using HotelManagementAPI.DTOs;

namespace HotelManagementAPI.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RegisterAsync(RegisterRequest request);
    string GenerateJwtToken(string username, string email, int userId);
}

