using BagApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BagApi.Data;

public class BagContext : IdentityDbContext<User>
{
    public BagContext(DbContextOptions<BagContext> options) : base(options)
    {
    }

    public DbSet<Bag> Bags => Set<Bag>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Brand>().HasData(
            new Brand { Id = 1, Name = "Zara" },
            new Brand { Id = 2, Name = "Gogoli" },
            new Brand { Id = 3, Name = "Shimimi" }
        );
    }
}