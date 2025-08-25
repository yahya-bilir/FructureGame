using PropertySystem;

namespace Events
{
    public class OnAllStationariesUpgraded
    {
        public PropertyQuery PropertyQuery { get; private set; }
        public float MultiplierValue { get; private set; }

        public OnAllStationariesUpgraded(PropertyQuery propertyQuery, float multiplierValue)
        {
            PropertyQuery = propertyQuery;
            MultiplierValue = multiplierValue;
        }
    }
}