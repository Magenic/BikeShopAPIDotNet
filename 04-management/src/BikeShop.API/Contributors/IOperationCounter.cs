using System.Collections.Generic;
using BikeShop.API.Models;

namespace BikeShop.API.Contributors
{
    public interface IOperationCounter<T>
    {
        void Increment(TrackedOperation operation);

        IDictionary<TrackedOperation, int> GetCounts { get; }

        string Name { get; }
    }
}