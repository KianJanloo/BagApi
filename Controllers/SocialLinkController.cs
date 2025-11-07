using System;
using BagApi.Data;
using BagApi.Dtos.SocialLinks;
using BagApi.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SocialLinkController: ControllerBase
{
    private readonly BagContext _dbContext;

    public SocialLinkController(BagContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var socialLinks = await _dbContext.SocialLinks
                                .Select(s => s.ToSocialLinkDto())
                                .AsNoTracking()
                                .ToListAsync();
        return Ok(socialLinks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var socialLink = await _dbContext.SocialLinks
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.Id == id);

        if (socialLink is null) return NotFound();
        return Ok(socialLink.ToSocialLinkDto());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSocialLinkDto dto)
    {
        if (await _dbContext.SocialLinks.AnyAsync(s => s.Name == dto.Name))
        {
            return Conflict("This social link already exists.");
        }

        var socialLink = dto.ToEntity();
        _dbContext.SocialLinks.Add(socialLink);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { Id = socialLink.Id }, socialLink.ToSocialLinkDto());
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSocialLinkDto dto)
    {
        var socialLink = await _dbContext.SocialLinks.FirstOrDefaultAsync(s => s.Id == id);

        if (socialLink is null)
        {
            return NotFound();
        }

        if (await _dbContext.SocialLinks.AnyAsync(s => s.Name == dto.Name))
        {
            return Conflict("This social link already exists.");
        }

        socialLink.Name = dto.Name;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var socialLink = await _dbContext.SocialLinks.FindAsync(id);

        if (socialLink is null)
        {
            return NotFound();
        }

        _dbContext.SocialLinks.Remove(socialLink);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
