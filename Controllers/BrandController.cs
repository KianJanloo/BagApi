using BagApi.Data;
using BagApi.Dtos.Brands;
using BagApi.Entities;
using BagApi.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly BagContext _dbContext;

        public BrandController(BagContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            string? search,
            string? sortBy = "Name",
            string? sortOrder = "asc",
            int page = 1,
            int limit = 10
        )
        {
            var query = _dbContext.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Name.Contains(search));
            }

            query = sortBy?.ToLower() switch
            {
                "name" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
                _ => sortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            };

            var brands = await _dbContext.Brands
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(b => b.ToBrandDto())
                .AsNoTracking()
                .ToListAsync();
            return Ok(new
            {
                TotalItems = await query.CountAsync(),
                Page = page,
                Items = brands
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _dbContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return NotFound();
            return Ok(brand.ToBrandDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            if (await _dbContext.Brands.AnyAsync(b => b.Name == dto.Name))
                return Conflict("This brand already exists.");

            var brand = dto.ToEntity();
            _dbContext.Brands.Add(brand);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand.ToBrandDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
        {
            var brand = await _dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return NotFound();

            brand.Name = dto.Name;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dbContext.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            _dbContext.Brands.Remove(brand);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
