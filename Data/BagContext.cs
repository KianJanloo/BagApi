
using BagApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Data;

public class BagContext(DbContextOptions<BagContext> options): DbContext(options)
{
    public DbSet<Bag> Bags => Set<Bag>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>().HasData(
            new { Id = 1, Name = "Zara" },
            new { Id = 2, Name = "Gogoli" },
            new { Id = 3, Name = "Shimimi" }
        );
    }

}
