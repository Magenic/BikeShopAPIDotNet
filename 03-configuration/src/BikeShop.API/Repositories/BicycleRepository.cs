using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Data;
using BikeShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeShop.API.Repositories
{
    public class BicycleRepository 
    { 
        private readonly BicycleDbContext _bicycleDbContext;

        public BicycleRepository(BicycleDbContext bicycleDbContext)
        {
            _bicycleDbContext = bicycleDbContext;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await _bicycleDbContext.Bicycles.ToListAsync();
        }

        public async Task<Bicycle> GetBicycle(long id)
        {
            return await _bicycleDbContext.FindAsync<Bicycle>(id);
        }

        public async Task<int> AddBicycle(Bicycle bicycle)
        {
            
            _bicycleDbContext.Add(bicycle);
            return await _bicycleDbContext.SaveChangesAsync();
        }
    }

}