using BikeShop.API.Models;
using Steeltoe.Management.Endpoint.Info;

namespace BikeShop.API.Contributors
{
    public class BicycleInfoContributor : IInfoContributor
    {
        private readonly IOperationCounter<Bicycle> _operationCounter;

        public BicycleInfoContributor(IOperationCounter<Bicycle> operationCounter)
        {
            _operationCounter = operationCounter;
        }

        public void Contribute(IInfoBuilder builder)
        {
            builder.WithInfo(
                _operationCounter.Name,
                _operationCounter.GetCounts
            );
        }
    }
}