using Microsoft.EntityFrameworkCore;
using UrlShortner.Api.Entities;

namespace UrlShortner.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<ShortUrl> ShortUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>().HasIndex(x => x.UniqueCode).IsUnique();
    }
    
}