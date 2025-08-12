using System.Collections.Generic;
using PropertySystem;
using Trains;

namespace Events
{
    public class OnTrainPropertyUpgraded
    {
        public PropertyQuery PropertyQuery { get; private set; }
        public List<TrainEngine> Engines { get; private set; }
        public float NewValue { get; private set; }

        public OnTrainPropertyUpgraded(PropertyQuery propertyQuery, float newValue, List<TrainEngine> engines)
        {
            PropertyQuery = propertyQuery;
            NewValue = newValue;
            Engines = engines;
        }
    }
}