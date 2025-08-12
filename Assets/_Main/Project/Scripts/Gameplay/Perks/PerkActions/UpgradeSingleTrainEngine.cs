using System.Collections.Generic;
using Events;
using PropertySystem;
using Trains;
using UnityEngine;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Upgrade Engine Perk")]

    public class UpgradeSingleTrainEngine : PerkAction
    {
        [SerializeField] private PropertyQuery propertyQuery;
        [SerializeField] private float newValue;
        [SerializeField] private List<TrainEngine> trainEngines;
        
        public override void Execute()
        {
            EventBus.Publish(new OnTrainPropertyUpgraded(propertyQuery, newValue, trainEngines));
        }
    }
}