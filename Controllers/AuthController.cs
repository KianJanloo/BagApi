using BagApi.Data;
using BagApi.Dtos.Auth;
using BagApi.Entities;
using BagApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly BagContext _db;
    private readonly JwtService _jwt;
    private readonly IEmailSender _emailSender;

    public AuthController(UserManager<User> userManager, BagContext db, JwtService jwt, IEmailSender emailSender)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);

        return Ok(new { message = "User registered successfully with role 'User'" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid credentials");

        var accessToken = await _jwt.GenerateAccessToken(user);
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

        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var newAccessToken = await _jwt.GenerateAccessToken(user);
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

    [HttpPost("forget-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Ok(new { message = "Email not found." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Auth",
            new { token, email = model.Email }, Request.Scheme);

        await _emailSender.SendEmailAsync(model.Email, "Reset Password",
            $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>");

        return Ok(new { message = "Reset link sent to your email." });
    }

    [HttpGet("reset-password")]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            return BadRequest("Invalid password reset token.");
        }
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Ok(new { message = "The email not found." });

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return Ok(new { message = "Password has been reset successfully." });
        }

        return BadRequest(result.Errors.Select(e => e.Description));
    }


}
