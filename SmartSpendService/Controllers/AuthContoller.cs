using Microsoft.AspNetCore.Mvc;
using SmartSpendService.DTOs;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email)    ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "All fields are required." });

        var (success, message) = _authService.Register(request);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "All fields are required." });

        var (success, message, data) = _authService.Login(request);

        if (!success)
            return Unauthorized(new { message });

        return Ok(data);
    }
}