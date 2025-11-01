using Microsoft.AspNetCore.Mvc;
using OfflineTicketingSystem.Data;
using OfflineTicketingSystem.DTOs;
using OfflineTicketingSystem.Services;
using System.Security.Cryptography;

namespace OfflineTicketingSystem.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _ctx;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext ctx, ITokenService tokenService)
    {
        _ctx = ctx;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest req)
    {
        var user = _ctx.Users.SingleOrDefault(u => u.Email == req.Email);
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(req.Password));
        if (!computed.SequenceEqual(user.PasswordHash)) return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse { Token = token, FullName = user.FullName, Email = user.Email, Role = user.Role.ToString() });
    }
}
