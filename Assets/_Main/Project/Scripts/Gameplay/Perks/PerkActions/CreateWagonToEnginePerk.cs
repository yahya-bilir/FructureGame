using EventBusses;
using Events;
using Trains;
using UnityEngine;
using VContainer;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Create Wagon Perk")]

    public class CreateWagonToEnginePerk : PerkAction
    {
        [SerializeField] private TrainEngine engine;
        [SerializeField] private int wagonCount;
        
        public override void Execute()
        {
            EventBus.Publish(new OnWagonCreationSelected(engine, wagonCount));
        }
    }
}