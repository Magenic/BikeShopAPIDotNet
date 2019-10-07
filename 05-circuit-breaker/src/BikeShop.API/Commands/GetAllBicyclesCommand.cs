using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;

namespace BikeShop.API.Commands
{
    public class GetAllBicyclesCommand : HystrixCommand<IEnumerable<Bicycle>>
    {

        private readonly BicycleRepository _bicycleRepository;
        private readonly ILogger<GetAllBicyclesCommand> _logger;

        public GetAllBicyclesCommand(IHystrixCommandOptions options, ILogger<GetAllBicyclesCommand> logger,
            BicycleRepository bicycleRepository) : base(options)
        {
            _bicycleRepository = bicycleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await ExecuteAsync();
        }

        protected override async Task<IEnumerable<Bicycle>> RunAsync()
        {
            _logger.LogInformation("GetAllBicyclesCommand.RunAsync()");
            // await Task.Delay(1500);
            return await _bicycleRepository.GetAllBicycles();
        }

        protected override async Task<IEnumerable<Bicycle>> RunFallbackAsync()
        {
            _logger.LogInformation("GetAllBicyclesCommand.RunFallbackAsync()");
            return await Task.FromResult<IEnumerable<Bicycle>>(new List<Bicycle>());
        }
    }
}
