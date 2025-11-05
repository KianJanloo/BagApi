using System;
using BagApi.Data;
using BagApi.Dtos.Brands;
using BagApi.Entities;
using BagApi.Mapping;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Endpoints;

public static class BrandsEndpoints
{
    const string GetBrandName = "getBrandName";

    public static RouteGroupBuilder MapBrandsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("brands");

        group.MapGet("/", async (BagContext dbContext) =>
        {
            return await dbContext.Brands
                            .Select(brand => brand.ToBrandDto())
                            .AsNoTracking()
                            .ToListAsync();
        });

        group.MapGet("/{id}", async (BagContext dbContext, int Id) =>
        {
            Brand? brand = await dbContext.Brands
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(b => b.Id == Id);

            return brand is null ? Results.NotFound() : Results.Ok(brand.ToBrandDto());
        }).WithName(GetBrandName);

        group.MapPost("/", async (BagContext dbContext, CreateBrandDto newBrand) =>
        {
            bool existingBrand = await dbContext.Brands.AnyAsync(b => b.Name == newBrand.Name);

            if (existingBrand)
            {
                return Results.Conflict("This brand already exists.");
            }

            Brand brand = newBrand.ToEntity();
            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetBrandName, new { Id = brand.Id }, brand.ToBrandDto());
        });

        group.MapPut("/{id}", async (BagContext dbContext, int id, UpdateBrandDto updatedBrand) =>
        {
            Brand? brand = await dbContext.Brands
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand is null)
                return Results.NotFound();

            brand.Name = updatedBrand.Name;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
        
        group.MapDelete("/{id}", async (BagContext dbContext, int Id) =>
        {
            Brand? brand = await dbContext.Brands.FindAsync(Id);

            if (brand is null)
            {
                return Results.NotFound();
            }

            await dbContext.Brands
                            .Where(brand => brand.Id == Id)
                            .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}
