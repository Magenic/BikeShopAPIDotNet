using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
//next line added in lab #5
using BikeShop.API.Commands;

namespace BikeShop.API.Services
{
    public class BicycleService
    {
        private readonly BicycleRepository _bicycleRepository;

        //next 2 lines added in lab #5
        private readonly GetAllBicyclesCommand _getAllBicyclesCommand;
        private readonly UpdateBicycleCommand _updateBicycleCommand;

        public BicycleService(BicycleRepository bicycleRepository, GetAllBicyclesCommand getAllBicyclesCommand, UpdateBicycleCommand updateBicycleCommand)
        {
            _bicycleRepository = bicycleRepository;
            //next 2 lines added in lab #5
            _getAllBicyclesCommand = getAllBicyclesCommand;
            _updateBicycleCommand = updateBicycleCommand;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            //return await _bicycleRepository.GetAllBicycles();
            //next line added in lab #5
            return await _getAllBicyclesCommand.GetAllBicycles();
        }

        public async Task<Bicycle> GetBicycle(long id)
        {
            return await _bicycleRepository.GetBicycle(id);
        }

        public async Task<int> AddBicycle(Bicycle bicycle)
        {
            return await _bicycleRepository.AddBicycle(bicycle);
        }

        public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
            return await _updateBicycleCommand.UpdateBicycle(bicycle);
        }

        public async Task<int> DeleteBicycle(long id)
        {
            return await _bicycleRepository.DeleteBicycle(id);
        }
    }
}