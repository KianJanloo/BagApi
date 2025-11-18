using BagApi.Data;
using BagApi.Dtos.Shoes;
using BagApi.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Controllers;

[ApiController]
[Route("api/shoes")]
public class ShoesController : ControllerBase
{
    private readonly BagContext _dbContext;

    public ShoesController(BagContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        string? search,
        string? BrandFilter,
        string sortBy = "Name",
        string sortOrder = "asc",
        int page = 1,
        int limit = 10
    )
    {
        var query = _dbContext.Shoes.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.Name.Contains(search));
        }

        if (!string.IsNullOrEmpty(BrandFilter))
        {
            query = query.Where(s => s.BrandId == int.Parse(BrandFilter));
        }

        query = sortBy.ToLower() switch
        {
            "name" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "price" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(s => s.Price) : query.OrderBy(s => s.Price),
            "createdat" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => sortOrder.ToLower() == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
        };

        int totalItems = await query.CountAsync();

        var shoes = await query.Skip((page - 1) * limit).Take(limit).AsNoTracking().ToListAsync();
        return Ok(new
        {
            total = totalItems,
            data = shoes.Select(s => ShoesMapping.ToShoesDetailDto(s))
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var shoe = await _dbContext.Shoes
            .Include(s => s.Brand)
            .Select(s => ShoesMapping.ToShoesDetailDto(s))
            .FirstOrDefaultAsync(s => s.Id == id);
        return Ok(shoe);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateShoesDto dto)
    {
        if (!await _dbContext.Brands.AnyAsync(b => b.Id == dto.BrandId))
        {
            return BadRequest("Brand not found");
        }
        var shoe = ShoesMapping.ToEntity(dto);

        shoe.CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

        _dbContext.Shoes.Add(shoe);

        await _dbContext.SaveChangesAsync();

        var createdShoes = await _dbContext.Shoes
            .Include(s => s.Brand)
            .Select(s => ShoesMapping.ToShoesDetailDto(s))
            .FirstOrDefaultAsync(s => s.Id == shoe.Id);

        return CreatedAtAction(nameof(Get), new { id = shoe.Id }, createdShoes);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShoesDto dto)
    {
        var shoe = await _dbContext.Shoes.FirstOrDefaultAsync(s => s.Id == id);
        if (shoe == null)
        {
            return NotFound("Shoes not found");
        }

        if (!await _dbContext.Brands.AnyAsync(b => b.Id == dto.BrandId))
        {
            return BadRequest("Brand not found");
        }

        shoe.Name = dto.Name;
        shoe.BrandId = dto.BrandId;
        shoe.Price = dto.Price;

        await _dbContext.SaveChangesAsync();

        return Ok(ShoesMapping.ToShoesDetailDto(shoe));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var shoe = await _dbContext.Shoes.FirstOrDefaultAsync(s => s.Id == id)
        ;
        if (shoe == null)
        {
            return NotFound("Shoes not found");
        }
        _dbContext.Shoes.Remove(shoe);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}