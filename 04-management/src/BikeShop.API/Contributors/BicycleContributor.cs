using System;
using System.Linq;
using BikeShop.API.Data;
using Steeltoe.Common.HealthChecks;

namespace BikeShop.API.Contributors
{
    public class BicycleContributor : IHealthContributor
    {
        private readonly BicycleDbContext _bicycleDbContext;

        public BicycleContributor(BicycleDbContext bicycleDbContext)
        {
            _bicycleDbContext = bicycleDbContext;
        }


        public HealthCheckResult Health()
        {
            var health = new HealthCheckResult();

            try
            {
                var count = _bicycleDbContext.Bicycles.ToList().Count;

                health.Status = HealthStatus.UP;
                health.Details.Add("count", count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                health.Status = HealthStatus.DOWN;
            }
            return health;
        }

        public string Id { get; } = "BicycleDb";
    }
}