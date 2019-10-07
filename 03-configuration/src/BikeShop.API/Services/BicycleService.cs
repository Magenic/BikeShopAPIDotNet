using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;

namespace BikeShop.API.Services
{
    public class BicycleService
    {
        private readonly BicycleRepository _bicycleRepository;

        public BicycleService(BicycleRepository bicycleRepository)
        {
            _bicycleRepository = bicycleRepository;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await _bicycleRepository.GetAllBicycles();
        }

        public async Task<Bicycle> GetBicycle(long id)
        {
            return await _bicycleRepository.GetBicycle(id);
        }

        public async Task<int> AddBicycle(Bicycle bicycle)
        {
            return await _bicycleRepository.AddBicycle(bicycle);
        }
    }
}