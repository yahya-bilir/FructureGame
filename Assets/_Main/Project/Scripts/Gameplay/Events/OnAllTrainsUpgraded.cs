using PropertySystem;

namespace Events
{
    public class OnAllTrainsUpgraded
    {
        public PropertyQuery PropertyQuery { get; private set; }
        public float MultiplierValue { get; private set; }

        public OnAllTrainsUpgraded(PropertyQuery propertyQuery, float multiplierValue)
        {
            PropertyQuery = propertyQuery;
            MultiplierValue = multiplierValue;
        }
    }
}