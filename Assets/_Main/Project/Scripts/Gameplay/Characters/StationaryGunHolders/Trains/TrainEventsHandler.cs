using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using PropertySystem;
using Trains;
using UnityEngine;
using VContainer;

namespace Characters.StationaryGunHolders.Trains
{
    public class TrainEventsHandler : IDisposable
    {
        private readonly List<TrainSystem> _trainSystems;
        private IEventBus _eventBus;

        public TrainEventsHandler(List<TrainSystem> trainSystems)
        {
            _trainSystems = trainSystems;
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus =  eventBus;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<OnEngineSelected>(HandleEngineSelected);
            _eventBus.Subscribe<OnWagonCreationSelected>(HandleWagonCreationSelected);
            _eventBus.Subscribe<OnTrainPropertyUpgraded>(HandleTrainPropertyUpgraded);
            _eventBus.Subscribe<OnAllTrainsUpgraded>(HandleAllTrainsUpgraded);
        }
        
        private void HandleEngineSelected(OnEngineSelected eventData)
        {
            OnEngineSelectedAsync(eventData).Forget();
        }

        private void HandleWagonCreationSelected(OnWagonCreationSelected eventData)
        {
            OnWagonCreationSelected(eventData).Forget();
        }

        private void HandleTrainPropertyUpgraded(OnTrainPropertyUpgraded eventData)
        {
            foreach (var sendEngine in eventData.Engines)
            {
                var engine = FindEngineWithPropertySO(sendEngine.CharacterPropertiesSO);
            
                if (engine == null) Debug.LogError("Engine is null");
            
                engine.CharacterPropertyManager.SetPropertyTemporarily(eventData.PropertyQuery, eventData.NewValue);
            
                foreach (var wagon in engine.Wagons)
                {
                    wagon.GunHolderPropertyManager.SetPropertyTemporarily(eventData.PropertyQuery, eventData.NewValue);
                }
                //TODO burada bir şey yap görsel olarak
            }

        }

        private void HandleAllTrainsUpgraded(OnAllTrainsUpgraded eventData)
        {
            foreach (var trainSystem in _trainSystems)
            {
                foreach (var engine in trainSystem.EngineInstances)
                {
                    var property = engine.CharacterPropertyManager.GetProperty(eventData.PropertyQuery);
                    engine.CharacterPropertyManager.SetPropertyTemporarily(eventData.PropertyQuery, property.TemporaryValue * eventData.MultiplierValue);
                    foreach (var wagon in engine.Wagons)
                    {
                        var wagonProperty = wagon.GunHolderPropertyManager.GetProperty(eventData.PropertyQuery);
                        wagon.GunHolderPropertyManager.SetPropertyTemporarily(eventData.PropertyQuery, wagonProperty.TemporaryValue * eventData.MultiplierValue);
                    }
                    //TODO burada bir şey yap görsel olarak
                }
            }
        }

        private async UniTask OnEngineSelectedAsync(OnEngineSelected eventData)
        {
            if (eventData.SystemIndex < 0 || eventData.SystemIndex >= _trainSystems.Count)
            {
                Debug.LogWarning($"Invalid system index: {eventData.SystemIndex}");
                return;
            }

            var system = _trainSystems[eventData.SystemIndex];
            
            await system.AddEngineToSystem(eventData.Engine);
        }

        private async UniTask OnWagonCreationSelected(OnWagonCreationSelected eventData)
        {
            var engine = FindEngineWithPropertySO(eventData.TrainEngine.CharacterPropertiesSO);

            if(engine == null) return;

            for (int i = 0; i < eventData.WagonCountToSpawn; i++)
            {
                engine.SpawnWagon();
                await UniTask.WaitForSeconds(0.15f);
            }
        }

        private TrainEngine FindEngineWithPropertySO(CharacterPropertiesSO propertiesSo)
        {
            TrainEngine engine = null;
            foreach (var trainSystem in _trainSystems)
            {
                foreach (var trainEngine in trainSystem.EngineInstances)
                {
                    if (trainEngine.CharacterPropertiesSO == propertiesSo)
                    {
                        engine = trainEngine;
                    }
                }
            }

            return engine;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnEngineSelected>(HandleEngineSelected);
            _eventBus.Unsubscribe<OnWagonCreationSelected>(HandleWagonCreationSelected);
            _eventBus.Unsubscribe<OnTrainPropertyUpgraded>(HandleTrainPropertyUpgraded);
            _eventBus.Unsubscribe<OnAllTrainsUpgraded>(HandleAllTrainsUpgraded);
        }
    }
}