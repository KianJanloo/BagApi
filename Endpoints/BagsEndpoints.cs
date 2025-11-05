using System;
using BagApi.Data;
using BagApi.Dtos.Bags;
using BagApi.Entities;
using BagApi.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Endpoints;

public static class BagsEndpoints
{

    const string GetBagName = "getBagName";

    public static RouteGroupBuilder MapBagsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("bags");

        group.MapGet("/", async (BagContext dbContext) =>
        {
            return await dbContext.Bags
                    .Include(bag => bag.Brand)
                    .Select(bag => bag.ToBagDto())
                    .AsNoTracking()
                    .ToListAsync();
        });

        group.MapGet("/{id}", async (BagContext dbContext, int Id) =>
        {
            Bag? bag = await dbContext.Bags.Include(b => b.Brand)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(b => b.Id == Id);

            return bag is null ? Results.NotFound() : Results.Ok(bag.ToBagDto());
        }).WithName(GetBagName);

        group.MapPost("/", async (BagContext dbContext, BagCreateDto createBag) =>
        {

            bool brandExists = await dbContext.Brands.AnyAsync(b => b.Id == createBag.BrandId);
            if (!brandExists)
                return Results.BadRequest("Brand not found");

            Bag bag = createBag.ToEntity();
            bag.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            dbContext.Bags.Add(bag);
            await dbContext.SaveChangesAsync();

            var createdBag = await dbContext.Bags
                .Include(b => b.Brand)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bag.Id);

            return Results.CreatedAtRoute(GetBagName, new { id = bag.Id }, bag.ToBagDto());
        });

        group.MapPut("/{id}", async (BagContext dbContext, int id, BagUpdateDto updateBag) =>
{
            Bag? bag = await dbContext.Bags
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (bag is null)
                return Results.NotFound();

            bag.Name = updateBag.Name;
            bag.Price = updateBag.Price;
            bag.BrandId = updateBag.BrandId;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (BagContext dbContext, int Id) =>
        {
            Bag? bag = await dbContext.Bags.FindAsync(Id);

            if (bag is null)
            {
                return Results.NotFound();
            }

            await dbContext.Bags
                            .Where(bag => bag.Id == Id)
                            .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }

}
