using EventBusses;
using Events;
using Trains;
using UnityEngine;
using VContainer;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Create Train Perk")]

    public class CreateTrainEnginePerk : PerkAction
    {
        [SerializeField] private TrainEngine engine;
        [SerializeField] private int lane;

        public override void Execute()
        {
            EventBus.Publish(new OnEngineSelected(engine, lane));
        }
    }
}