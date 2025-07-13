using System;
using System.Collections.Generic;
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
        private readonly IslandManager _islandManager;
        private readonly AstarPath _astarPath;
        private readonly RateChanger _rateChanger;
        private readonly Scaler _scaler;

        public IslandOpeningSystem(IslandCameraMovementManager islandCameraMovementManager, RateChanger rateChanger,
            Scaler scaler, IEventBus eventBus, Island island, List<GameObject> collidersToDisableWhenSelected,
            IslandJumpingActions jumpingActions,
            CloudMovementManager cloudMovementManager, IslandCharactersController islandCharactersController,
            List<IslandCloud> islandClouds, IslandManager islandManager, AstarPath astarPath)
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
            _islandManager = islandManager;
            _astarPath = astarPath;
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
            _collidersToDisableWhenSelected.ForEach(i => i.SetActive(false));
            _astarPath.Scan();
            await UniTask.WaitForSeconds(0.05f);

            _islandCameraMovementManager.OnIslandSelected().Forget();
            
            if(_island != _islandManager.firstIsland)
            {
                Debug.Log("waiting for characters to get into jumpiong pos");
                await UniTask.WaitForSeconds(0.25f);
                await _islandJumpingActions.WaitForCharactersToGetIntoJumpingPosition();
                //await _islandCameraMovementManager.GoToMainPositionForCloudOpening();
            } 
            //await _cloudMovementManager.StartCloudActions();

            await _scaler.SpawnIslandObjects();
            //_scaler.ActivateObjects();
            _islandCharactersController.ActivateSections().Forget();

            // await UniTask.WhenAll(
            //     _islandClouds.ConvertAll(cloud => cloud.OpenCloud())
            // );

            //_rateChanger.FadeOutRateOverTime();
            //await UniTask.WaitForSeconds(1f);
            //_scaler.ScaleUp().Forget();
            //await UniTask.WaitForSeconds(1f);
            


            if(_island != _islandManager.firstIsland)
            {
                Debug.Log("waiting for characters to jump");
                _islandCameraMovementManager.OnIslandOpenedCompletely().Forget();
                await _islandJumpingActions.MakeCharacterJump();
                await _islandJumpingActions.WaitForCharacterJumps();

                //await _islandJumpingActions.WaitForCharactersToGetIntoJumpingPosition();
            }

            //Debug.Log("Sections will be activated");
            // var rng = new System.Random();
            // rng.Shuffle(_openingSection);


            _eventBus.Publish(new OnIslandStarted(_island));
        }
    }
}