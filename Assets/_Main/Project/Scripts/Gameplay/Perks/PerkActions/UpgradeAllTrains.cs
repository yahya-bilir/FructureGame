using Events;
using PropertySystem;
using UnityEngine;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Upgrade All Trains Perk")]
    public class UpgradeAllTrains : PerkAction
    {
        [SerializeField] private PropertyQuery propertyQuery;
        [SerializeField] private float multiplierValue;
        public override void Execute()
        {
            EventBus.Publish(new OnAllTrainsUpgraded(propertyQuery, multiplierValue));
        }
    }
}