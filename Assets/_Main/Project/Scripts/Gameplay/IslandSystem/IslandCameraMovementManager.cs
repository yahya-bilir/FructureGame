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

        public IslandCameraMovementManager(Transform mainCameraPosition, CamerasManager camerasManager,
            IEventBus eventBus, Island island, Transform cardSelectionCameraPosition, IslandManager islandManager)
        {
            _mainCameraPosition = mainCameraPosition;
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _island = island;
            _cardSelectionCameraPosition = cardSelectionCameraPosition;
            _islandManager = islandManager;
        }

        public void Initialize()
        {
            _eventBus.Subscribe<OnAllClickablesClicked>(OnAllClickablesClicked);
        }

        private void OnAllClickablesClicked(OnAllClickablesClicked eventData)
        {
            if(_islandManager.CurrentIsland != _island) return;
            Debug.Log("camera happened");
            _camerasManager.MoveCameraToPos(_cardSelectionCameraPosition.position).Forget();
        }

        public void OnIslandSelected()
        {
            _camerasManager.MoveCameraToPos(_mainCameraPosition.position).Forget();
            _camerasManager.ToggleLensSize(8f);
        }
        
        
        public void Dispose()
        {
            _eventBus.Unsubscribe<OnAllClickablesClicked>(OnAllClickablesClicked);

        }
    }
}