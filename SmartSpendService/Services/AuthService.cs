using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartSpendService.DTOs;
using SmartSpendService.Helpers;
using SmartSpendService.Models;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class AuthService
{
    private readonly UserLibrary _userLibrary;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserLibrary userLibrary, IOptions<JwtSettings> jwtSettings)
    {
        _userLibrary = userLibrary;
        _jwtSettings = jwtSettings.Value;
    }

    public (bool Success, string Message) Register(RegisterRequest request)
    {
        // Check if email already exists
        if (_userLibrary.GetByEmail(request.Email) != null)
            return (false, "Email already registered.");

        // Check if username already exists
        if (_userLibrary.GetByUsername(request.Username) != null)
            return (false, "Username already taken.");

        // Hash password and create user
        var user = new User
        {
            Username     = request.Username,
            Email        = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt    = DateTime.UtcNow
        };

        _userLibrary.Create(user);
        return (true, "Registration successful.");
    }

    public (bool Success, string Message, AuthResponse? Data) Login(LoginRequest request)
    {
        // Check if user exists
        var user = _userLibrary.GetByUsername(request.Username);
        if (user == null)
            return (false, "Invalid email or password.", null);

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        // Generate JWT token
        var token = GenerateToken(user);

        var response = new AuthResponse
        {
            Token    = token,
            Username = user.Username,
            Email    = user.Email
        };

        return (true, "Login successful.", response);
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Email,          user.Email)
        };

        var token = new JwtSecurityToken(
            issuer:             _jwtSettings.Issuer,
            audience:           _jwtSettings.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddDays(_jwtSettings.ExpiryInDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}