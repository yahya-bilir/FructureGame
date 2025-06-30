using System;
using System.Collections.Generic;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events.IslandEvents;
using UnityEngine;
using VisualEffects;

namespace IslandSystem
{
    public class IslandOpeningSystem : IDisposable
    {
        private readonly CamerasManager _camerasManager;
        private readonly RateChanger _rateChanger;
        private readonly GameObject _enemiesContainer;
        private readonly Scaler _scaler;
        private readonly Transform _cameraPositioner;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private readonly List<GameObject> _collidersToDisableWhenSelected;

        public IslandOpeningSystem(CamerasManager camerasManager,
            RateChanger rateChanger, GameObject enemiesContainer, Scaler scaler, Transform cameraPositioner,
            IEventBus eventBus, Island island, List<GameObject> collidersToDisableWhenSelected)
        {
            _camerasManager = camerasManager;
            _rateChanger = rateChanger;
            _enemiesContainer = enemiesContainer;
            _scaler = scaler;
            _cameraPositioner = cameraPositioner;
            _eventBus = eventBus;
            _island = island;
            _collidersToDisableWhenSelected = collidersToDisableWhenSelected;
        }

        public void Initialize()
        {
            _enemiesContainer.SetActive(false);
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);

        }
        
        private void OnIslandSelected(OnIslandSelected eventData)
        {
            if(eventData.SelectedIsland != _island) return;
            OpenIslandUp().Forget();
        }

        private async UniTask OpenIslandUp()
        {
            await _camerasManager.MoveCameraToPos(_cameraPositioner.position);
            _rateChanger.FadeOutRateOverTime();
            await UniTask.WaitForSeconds(1f);
            await _scaler.ScaleUp();
            _collidersToDisableWhenSelected.ForEach(i => i.SetActive(false));
            if(_enemiesContainer != null) _enemiesContainer.SetActive(true);
            _eventBus.Publish(new OnIslandStarted(_island));
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }
    }
}