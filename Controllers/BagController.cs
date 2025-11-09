using BagApi.Data;
using BagApi.Dtos.Bags;
using BagApi.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BagController : ControllerBase
    {
        private readonly BagContext _dbContext;

        public BagController(BagContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            string? search,
            string? BrandFilter,
            string sortBy = "Name",
            string sortOrder = "asc",
            int page = 1,
            int limit = 10
        )
        {
            var query = _dbContext.Bags.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Name.Contains(search));
            }

            if (!string.IsNullOrEmpty(BrandFilter))
            {
                query = query.Include(b => b.Brand)
                             .Where(b => b.Brand!.Name == BrandFilter);
            }
            else
            {
                query = query.Include(b => b.Brand);
            }

            query = sortBy.ToLower() switch
            {
                "name" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
                "price" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                "createdat" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
                _ => sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
            };

            int totalItems = await query.CountAsync();

            var bags = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
            return Ok(new
            {
                TotalItems = totalItems,
                Page = page,
                Items = bags.ConvertAll(b => b.ToBagDto())
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var bag = await _dbContext.Bags
                .Include(b => b.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (bag == null) return NotFound();
            return Ok(bag.ToBagDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] BagCreateDto dto)
        {
            if (!await _dbContext.Brands.AnyAsync(b => b.Id == dto.BrandId))
                return BadRequest("Brand not found");

            var bag = dto.ToEntity();
            bag.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            _dbContext.Bags.Add(bag);
            await _dbContext.SaveChangesAsync();

            var createdBag = await _dbContext.Bags
                .Include(b => b.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bag.Id);

            return CreatedAtAction(nameof(Get), new { id = bag.Id }, bag.ToBagDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] BagUpdateDto dto)
        {
            var bag = await _dbContext.Bags.Include(b => b.Brand).FirstOrDefaultAsync(b => b.Id == id);
            if (bag == null) return NotFound();

            bag.Name = dto.Name;
            bag.Price = dto.Price;
            bag.BrandId = dto.BrandId;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var bag = await _dbContext.Bags.FindAsync(id);
            if (bag == null) return NotFound();

            _dbContext.Bags.Remove(bag);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
