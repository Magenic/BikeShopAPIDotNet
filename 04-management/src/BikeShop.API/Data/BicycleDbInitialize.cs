using System;
using System.Linq;
using BikeShop.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BikeShop.API.Data
{
    public static class BicycleDbInitialize
    {
        public static void init(IServiceProvider serviceProvider)
        {
            using (var context = new BicycleDbContext(serviceProvider.GetRequiredService<DbContextOptions<BicycleDbContext>>()))
            {
                context.Database.EnsureCreated();
                
                if (context.Bicycles.Any()) return;
                
                context.Bicycles.Add(new Bicycle
                {
                    Description = "Schwin",
                    Image = "img",
                    Price = 899.99,
                    ProductName = "Schwin Mountian Bike"
                });
                context.Bicycles.Add(new Bicycle
                {
                    Description = "Nishiki",
                    Image = "img",
                    Price = 399.99,
                    ProductName = "Nishiki Dirt Bike"
                });
                context.SaveChanges();
            }
        }
    }
}