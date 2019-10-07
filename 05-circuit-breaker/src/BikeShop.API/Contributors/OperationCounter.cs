using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BikeShop.API.Models;

namespace BikeShop.API.Contributors
{
    public class OperationCounter<T> : IOperationCounter<T>
    {
        private readonly IDictionary<TrackedOperation, int> _count;

        public OperationCounter()
        {
            _count = new Dictionary<TrackedOperation, int>();

            foreach (var action in Enum.GetValues(typeof(TrackedOperation)))
            {
                _count.Add((TrackedOperation) action, 0);
            }
        }

        public void Increment(TrackedOperation operation)
        {
            _count[operation] = ++_count[operation];
        }

        public IDictionary<TrackedOperation, int> GetCounts => _count.ToImmutableDictionary();
        public string Name => $"{typeof(T).Name}Operations";
    }
}