using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;

namespace BikeShop.API.Commands
{
    public class UpdateBicycleCommand : HystrixCommand<int>
    {

        private readonly BicycleRepository _bicycleRepository;
        private readonly ILogger<UpdateBicycleCommand> _logger;
        private Bicycle _bicycle;

        public UpdateBicycleCommand(IHystrixCommandOptions options, ILogger<UpdateBicycleCommand> logger, BicycleRepository bicycleRepository) : base(options)
        {
            _bicycleRepository = bicycleRepository;
            _logger = logger;
        }

        public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
            _bicycle = bicycle;
            return await ExecuteAsync();
        }

        protected override async Task<int> RunAsync()
        {
            _logger.LogInformation("UpdateBicycleCommand.RunAsync()");
            return await _bicycleRepository.UpdateBicycle(_bicycle);
        }

        protected override async Task<int> RunFallbackAsync()
        {
            _logger.LogInformation("UpdateBicycleCommand.RunFallbackAsync()");
            return await Task.FromResult<int>(0);
        }
    }
}
