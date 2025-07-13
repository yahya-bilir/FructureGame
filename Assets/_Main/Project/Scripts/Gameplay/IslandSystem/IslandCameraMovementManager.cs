using System;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using Events.IslandEvents;
using UnityEngine;

namespace IslandSystem
{
    public class IslandCameraMovementManager : IDisposable
    {
        private readonly Transform _mainCameraPosition;
        private readonly CamerasManager _camerasManager;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private readonly Transform _cardSelectionCameraPosition;
        private readonly IslandManager _islandManager;
        private readonly Transform _openingCameraPosition;

        public IslandCameraMovementManager(Transform mainCameraPosition, CamerasManager camerasManager,
            IEventBus eventBus, Island island, Transform cardSelectionCameraPosition, IslandManager islandManager,
            Transform openingCameraPosition)
        {
            _mainCameraPosition = mainCameraPosition;
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _island = island;
            _cardSelectionCameraPosition = cardSelectionCameraPosition;
            _islandManager = islandManager;
            _openingCameraPosition = openingCameraPosition;
        }

        public void Initialize()
        {
            _eventBus.Subscribe<OnAllClickablesClicked>(OnAllClickablesClicked);
        }

        private void OnAllClickablesClicked(OnAllClickablesClicked eventData)
        {
            if(_islandManager.CurrentIsland != _island) return;
            Debug.Log("camera happened");
            _camerasManager.ToggleLensSize(6.49f);
            //_camerasManager.ToggleLensSize(5f);

            _camerasManager.MoveCameraToPos(_cardSelectionCameraPosition.position).Forget();
        }

        public async UniTask OnIslandSelected()
        {
            _camerasManager.ToggleLensSize(6.49f);
            //_camerasManager.ToggleLensSize(5f);
            await _camerasManager.MoveCameraToPos(_openingCameraPosition.position, 3.5f);
        }        
        
        public async UniTask OnIslandOpenedCompletely()
        {
            _camerasManager.ToggleLensSize(5f);
            await _camerasManager.MoveCameraToPos(_mainCameraPosition.position);
        }        
        
        public async UniTask GoToMainPositionForCloudOpening()
        {
            await _camerasManager.MoveCameraToPos(_mainCameraPosition.position);
        }
        
        
        public void Dispose()
        {
            _eventBus.Unsubscribe<OnAllClickablesClicked>(OnAllClickablesClicked);

        }
    }
}