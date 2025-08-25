using System;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters.StationaryGunHolders
{
    public class GunHolderEventHandler : IDisposable
    {
        private readonly StationaryGunHolderCharacter _stationaryGunHolderCharacter;
        private readonly CharacterPropertyManager _characterPropertyManager;
        private IEventBus _eventBus;

        public GunHolderEventHandler(StationaryGunHolderCharacter stationaryGunHolderCharacter, CharacterPropertyManager characterPropertyManager)
        {
            _stationaryGunHolderCharacter = stationaryGunHolderCharacter;
            _characterPropertyManager = characterPropertyManager;
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<OnAllStationariesUpgraded>(HandleOnAllStationariesUpgraded);
        }
        
        private void HandleOnAllStationariesUpgraded(OnAllStationariesUpgraded eventData)
        {
            var property = _characterPropertyManager.GetProperty(eventData.PropertyQuery);
            _characterPropertyManager.SetPropertyTemporarily(eventData.PropertyQuery, property.TemporaryValue * eventData.MultiplierValue);
            Debug.Log($"Property upgraded: {eventData.PropertyQuery} | Value: {property.TemporaryValue} | Name: {_stationaryGunHolderCharacter.name}");
            //TODO burada bir şey yap görsel olarak
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnAllStationariesUpgraded>(HandleOnAllStationariesUpgraded);
        }
    }
}