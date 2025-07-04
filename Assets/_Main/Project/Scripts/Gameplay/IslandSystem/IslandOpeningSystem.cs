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
        private readonly RateChanger _rateChanger;
        private readonly Scaler _scaler;

        public IslandOpeningSystem(IslandCameraMovementManager islandCameraMovementManager, RateChanger rateChanger,
            Scaler scaler, IEventBus eventBus, Island island, List<GameObject> collidersToDisableWhenSelected, IslandJumpingActions jumpingActions,
            CloudMovementManager cloudMovementManager, IslandCharactersController islandCharactersController)
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
            await _cloudMovementManager.StartCloudActions();
            _islandCameraMovementManager.OnIslandSelected();
            _scaler.ActivateObjects();
            //_rateChanger.FadeOutRateOverTime();
            //await UniTask.WaitForSeconds(1f);
            await _scaler.ScaleUp();
            _collidersToDisableWhenSelected.ForEach(i => i.SetActive(false));
            await _islandJumpingActions.WaitForCharacterJumps();
            
            // var rng = new System.Random();
            // rng.Shuffle(_openingSection);

            await _islandCharactersController.ActivateSections();

            _eventBus.Publish(new OnIslandStarted(_island));
        }
    }
}