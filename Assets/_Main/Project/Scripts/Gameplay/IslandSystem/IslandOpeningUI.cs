using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using Events.IslandEvents;
using UnityEngine;
using VContainer;

namespace IslandSystem
{
    public class IslandOpeningUI : MonoBehaviour 
    {
        private bool _availableToOpen;
        private IEventBus _eventBus;
        private Island _island;
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void MakeIslandAvailable()
        {
            transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                _availableToOpen = true;
            });
        }

        public void Initialize(Island island)
        {
            _island = island;
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            DisableIslandOpeningUI().Forget();
        }
        

        public async UniTask StartIslandOpeningActions()
        {
            await DisableIslandOpeningUI();
            _eventBus.Publish(new OnIslandSelected(_island));
        }

        private async UniTask DisableIslandOpeningUI()
        {
            _availableToOpen = false;
            transform.DOScale(Vector3.zero, 0.2f);
            await UniTask.WaitForSeconds(0.2f);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }

        public void OnMouseDown()
        {
            if (!_availableToOpen) return;
            Debug.Log("Mouse down");
            StartIslandOpeningActions().Forget();
        }
    }
}