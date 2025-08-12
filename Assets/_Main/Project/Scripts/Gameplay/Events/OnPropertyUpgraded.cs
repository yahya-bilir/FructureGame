using PropertySystem;

namespace Events
{
    public class OnPropertyUpgraded
    {
        public CharacterPropertyManager CharacterPropertyManager { get; private set; }
        public PropertyQuery PropertyQuery { get; private set; }
        public OnPropertyUpgraded(CharacterPropertyManager characterPropertyManager, PropertyQuery propertyQuery)
        {
            CharacterPropertyManager = characterPropertyManager;
            PropertyQuery = propertyQuery;
        }
    }
}