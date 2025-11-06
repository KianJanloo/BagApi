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
        public async Task<IActionResult> GetAll()
        {
            var brands = await _dbContext.Brands
                .Select(b => b.ToBrandDto())
                .AsNoTracking()
                .ToListAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
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
