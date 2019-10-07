using System;
using Steeltoe.Common.HealthChecks;

namespace BikeShop.API.Contributors
{
    public class DemoContributor : IHealthContributor
    {
        public DemoContributor() { }

        public HealthCheckResult Health()
        {
            var health = new HealthCheckResult();
            try
            {
                //toggle status between UP and WARNING every minute
                var inGoodShape = (DateTime.UtcNow.Minute % 2) == 0;
                health.Status = inGoodShape ? HealthStatus.UP : HealthStatus.WARNING;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                health.Status = HealthStatus.DOWN;
            }
            health.Details.Add("status", health.Status.ToString());
            return health;
        }

        public string Id { get; } = "Demo";
    }
}