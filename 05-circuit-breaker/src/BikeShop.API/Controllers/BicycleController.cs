using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Services;
using Microsoft.AspNetCore.Mvc;
// added in lab #4
using BikeShop.API.Contributors;

namespace BikeShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicycleController : ControllerBase
    {
        private readonly BicycleService _bicycleService;

        private readonly IOperationCounter<Bicycle> _operationCounter;

        public BicycleController(BicycleService bicycleService, IOperationCounter<Bicycle> operationCounter)
        {
            _bicycleService = bicycleService;
            _operationCounter = operationCounter;
        }

        [HttpGet]
        public async Task<IEnumerable<Bicycle>> Get()
        {
            return await _bicycleService.GetAllBicycles();
        }

        // public IDictionary<TrackedOperation, int> GetCounts => _count.ToImmutableDictionary();

        [HttpGet("{id}")]
        public async Task<Bicycle> Get(long id)
        {
            _operationCounter.Increment(TrackedOperation.Get);
            return await _bicycleService.GetBicycle(id);
        }

        [HttpPost]
        public async Task<int> Add([FromBody] Bicycle bicycle)
        {
            _operationCounter.Increment(TrackedOperation.Add);
            return await _bicycleService.AddBicycle(bicycle);
        }

        [HttpPut]
        public async Task<int> Update([FromBody] Bicycle bicycle)
        {
            _operationCounter.Increment(TrackedOperation.Update);
            return await _bicycleService.UpdateBicycle(bicycle);
        }

        [HttpDelete]
        public async Task<int> Delete(long id)
        {
            _operationCounter.Increment(TrackedOperation.Delete);
            return await _bicycleService.DeleteBicycle(id);
        }
    }
}