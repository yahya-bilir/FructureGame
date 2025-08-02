using System.Collections.Generic;
using Perks.PerkActions;
using PerkSystem;

namespace Events
{
    public class OnLevelUpgraded
    {
        public List<PerkAction> PerkActions { get; private set; }
        
        public OnLevelUpgraded(List<PerkAction> perkActions)
        {
            PerkActions = perkActions;
        }
    }
}