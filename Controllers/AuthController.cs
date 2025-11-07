using BagApi.Data;
using BagApi.Dtos.Auth;
using BagApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly BagContext _db;
    private readonly JwtService _jwt;

    public AuthController(UserManager<User> userManager, BagContext db, JwtService jwt)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid credentials");

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", user.Id);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var oldToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

        if (oldToken == null || !oldToken.IsActive)
            return Unauthorized("Invalid refresh token");

        var user = await _userManager.FindByIdAsync(oldToken.UserId);
        if (user == null) return Unauthorized();

        // Revoke old token
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Generate new tokens
        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken(
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            user.Id
        );

        oldToken.ReplacedByToken = newRefreshToken.Token;

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        });
    }

}
