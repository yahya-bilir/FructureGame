using System;
using System.Collections.Generic;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events.IslandEvents;
using UnityEngine;
using Utilities;
using VisualEffects;

namespace IslandSystem
{
    public class IslandOpeningSystem : IDisposable
    {
        private readonly Transform _cameraPositioner;
        private readonly CamerasManager _camerasManager;
        private readonly List<GameObject> _collidersToDisableWhenSelected;
        private readonly List<OpeningSection> _openingSection;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private readonly IslandJumpingActions _islandJumpingActions;
        private readonly RateChanger _rateChanger;
        private readonly Scaler _scaler;

        public IslandOpeningSystem(CamerasManager camerasManager, RateChanger rateChanger,
            List<OpeningSection> openingSection, Scaler scaler, Transform cameraPositioner, IEventBus eventBus,
            Island island, List<GameObject> collidersToDisableWhenSelected, IslandJumpingActions jumpingActions)
        {
            _camerasManager = camerasManager;
            _rateChanger = rateChanger;
            _openingSection = openingSection;
            _scaler = scaler;
            _cameraPositioner = cameraPositioner;
            _eventBus = eventBus;
            _island = island;
            _collidersToDisableWhenSelected = collidersToDisableWhenSelected;
            _islandJumpingActions = jumpingActions;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }


        public void Initialize()
        {
            foreach (var section in _openingSection)
            {
                foreach (var enemy in section.Enemies)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            if (eventData.SelectedIsland != _island) return;
            OpenIslandUp().Forget();
        }

        private async UniTask OpenIslandUp()
        {
            await _camerasManager.MoveCameraToPos(_cameraPositioner.position);
            _rateChanger.FadeOutRateOverTime();
            await UniTask.WaitForSeconds(1f);
            await _scaler.ScaleUp();
            _collidersToDisableWhenSelected.ForEach(i => i.SetActive(false));
            await _islandJumpingActions.WaitForCharacterJumps();
            
            // var rng = new System.Random();
            // rng.Shuffle(_openingSection);
            
            foreach (var section in _openingSection)
            {
                foreach (var enemy in section.Enemies)
                {
                    enemy.gameObject.SetActive(true);
                    enemy.CharacterVisualEffects.SpawnCharacter();
                }
                await UniTask.WaitForSeconds(0.25f);
            }

            _eventBus.Publish(new OnIslandStarted(_island));
        }
    }
}