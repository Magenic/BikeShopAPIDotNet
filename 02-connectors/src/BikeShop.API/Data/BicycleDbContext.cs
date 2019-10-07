using BikeShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeShop.API.Data
{
    public class BicycleDbContext : DbContext
    {
        public DbSet<Bicycle> Bicycles { get; set; }

        public BicycleDbContext(DbContextOptions<BicycleDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);
    
          modelBuilder.Entity<Bicycle>(entity =>
          {
              entity.HasKey(e => e.Id);
              entity.Property(e => e.ProductName);
              entity.Property(e => e.Description);
              entity.Property(e => e.Price);
              entity.Property(e => e.Image);
          });
        }
    }
}