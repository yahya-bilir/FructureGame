using Events;
using PropertySystem;
using UnityEngine;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Upgrade All Stationaries Perk")]
    public class UpgradeAllStationaries : PerkAction
    {
        [SerializeField] private PropertyQuery propertyQuery;
        [SerializeField] private float multiplierValue;
        public override void Execute()
        {
            EventBus.Publish(new OnAllStationariesUpgraded(propertyQuery, multiplierValue));
        }
    }
}