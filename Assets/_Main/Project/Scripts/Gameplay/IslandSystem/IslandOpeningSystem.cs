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
        private readonly IslandCameraMovementManager _islandCameraMovementManager;
        private readonly List<GameObject> _collidersToDisableWhenSelected;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private readonly IslandJumpingActions _islandJumpingActions;
        private readonly CloudMovementManager _cloudMovementManager;
        private readonly IslandCharactersController _islandCharactersController;
        private readonly List<IslandCloud> _islandClouds;
        private readonly RateChanger _rateChanger;
        private readonly Scaler _scaler;

        public IslandOpeningSystem(IslandCameraMovementManager islandCameraMovementManager, RateChanger rateChanger,
            Scaler scaler, IEventBus eventBus, Island island, List<GameObject> collidersToDisableWhenSelected,
            IslandJumpingActions jumpingActions,
            CloudMovementManager cloudMovementManager, IslandCharactersController islandCharactersController,
            List<IslandCloud> islandClouds)
        {
            _islandCameraMovementManager = islandCameraMovementManager;
            _rateChanger = rateChanger;
            _scaler = scaler;
            _eventBus = eventBus;
            _island = island;
            _collidersToDisableWhenSelected = collidersToDisableWhenSelected;
            _islandJumpingActions = jumpingActions;
            _cloudMovementManager = cloudMovementManager;
            _islandCharactersController = islandCharactersController;
            _islandClouds = islandClouds;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }
        
        public void Initialize()
        {
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            if (eventData.SelectedIsland != _island) return;
            OpenIslandUp().Forget();
        }

        private async UniTask OpenIslandUp()
        {
            _islandJumpingActions.WaitForCharactersToGetIntoJumpingPosition().Forget();
            await _islandCameraMovementManager.OnIslandSelected();
            //await _cloudMovementManager.StartCloudActions();
            await UniTask.WhenAll(
                _islandClouds.ConvertAll(cloud => cloud.OpenCloud())
            );
            _scaler.ActivateObjects();
            //_rateChanger.FadeOutRateOverTime();
            //await UniTask.WaitForSeconds(1f);
            await _scaler.ScaleUp();
            await UniTask.WaitForSeconds(1f);
            await _islandJumpingActions.WaitForCharacterJumps();
            //_collidersToDisableWhenSelected.ForEach(i => i.SetActive(false));
            Debug.Log("Sections will be activated");
            // var rng = new System.Random();
            // rng.Shuffle(_openingSection);

            await _islandCharactersController.ActivateSections();

            _eventBus.Publish(new OnIslandStarted(_island));
        }
    }
}