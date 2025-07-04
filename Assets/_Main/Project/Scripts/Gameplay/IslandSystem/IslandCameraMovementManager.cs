using System;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using UnityEngine;

namespace IslandSystem
{
    public class IslandCameraMovementManager : IDisposable
    {
        private readonly Transform _cameraPositioner;
        private readonly CamerasManager _camerasManager;
        private readonly IEventBus _eventBus;
        private readonly Island _island;

        public IslandCameraMovementManager(Transform cameraPositioner, CamerasManager camerasManager,
            IEventBus eventBus, Island island)
        {
            _cameraPositioner = cameraPositioner;
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _island = island;
        }

        public void Initialize()
        {
            _eventBus.Subscribe<OnAllIslandEnemiesKilled>(OnAllIslandEnemiesKilled);
        }

        private void OnAllIslandEnemiesKilled(OnAllIslandEnemiesKilled eventData)
        {
            if(eventData.Island != _island) return;
            
        }

        public void OnIslandSelected()
        {
            _camerasManager.MoveCameraToPos(_cameraPositioner.position).Forget();
            _camerasManager.ToggleLensSize(8f);
        }
        
        
        public void Dispose()
        {
            _eventBus.Unsubscribe<OnAllIslandEnemiesKilled>(OnAllIslandEnemiesKilled);

        }
    }
}