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
    public async Task<IActionResult> Register([FromBody] LoginDto dto)
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
        var refreshToken = _jwt.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var token = await _db.RefreshTokens
            .Include(rt => rt.UserId)
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

        if (token == null || !token.IsActive)
            return Unauthorized("Invalid refresh token");

        // revoke old token
        token.IsRevoked = true;

        var user = await _userManager.FindByIdAsync(token.UserId);
        if (user == null) return Unauthorized();

        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}
