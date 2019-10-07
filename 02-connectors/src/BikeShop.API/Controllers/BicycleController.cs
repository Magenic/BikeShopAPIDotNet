using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicycleController : ControllerBase
    {
        private readonly BicycleService _bicycleService;

        public BicycleController(BicycleService bicycleService)
        {
            _bicycleService = bicycleService;
        }

        [HttpGet]
        public async Task<IEnumerable<Bicycle>> Get()
        {
            return await _bicycleService.GetAllBicycles();
        }

        [HttpGet("{id}")]
        public async Task<Bicycle> Get(long id)
        {
            return await _bicycleService.GetBicycle(id);
        }

        [HttpPost]
        public async Task<int> Add([FromBody] Bicycle bicycle)
        {
            return await _bicycleService.AddBicycle(bicycle);
        }
    }
}