using System;
using System.Collections.Generic;
using CommonComponents;
using EventBusses;
using Events.IslandEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VisualEffects;

namespace IslandSystem
{
    public class Island : MonoBehaviour
    {
        private Scaler _scaler;
        private RateChanger _rateChanger;
        private IslandOpeningUI _islandOpeningUI;
        private CamerasManager _camerasManager;
        private IEventBus _eventBus;
        private IObjectResolver _resolver;
        
        [SerializeField] private List<IslandsAndColliders> nextIslandsToBeAvailable;
        [SerializeField] private Transform cameraPositioner;
        [SerializeField] private GameObject enemiesContainer;

        private IslandOpeningSystem _islandOpeningSystem;
        
        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus, IObjectResolver resolver)
        {
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _resolver = resolver;
        }

        private void Awake()
        {
            _scaler = GetComponentInChildren<Scaler>();
            _rateChanger = GetComponentInChildren<RateChanger>();
            _islandOpeningUI = GetComponentInChildren<IslandOpeningUI>();
        }

        private void Start()
        {
            _islandOpeningSystem = new IslandOpeningSystem(_camerasManager, _rateChanger,
                enemiesContainer, _scaler, cameraPositioner, _eventBus, this);
            
            _islandOpeningSystem.Initialize();
            _resolver.Inject(_islandOpeningUI);
            _islandOpeningUI.Initialize(this);
        }

        [Button]
        private void OnIslandFinished()
        {
            _eventBus.Publish(new OnIslandFinished(this));
        }
        
        public void MakeIslandAvailable()
        {
            _islandOpeningUI.MakeIslandAvailable();
        }
        
        public void StartIslandOpeningActions()
        {
            _islandOpeningUI.StartIslandOpeningActions();
        }
        
    }

    [Serializable]
    public struct IslandsAndColliders
    {
        [field: SerializeField] public Island IslandToBeAvailable { get; private set; }
        [field: SerializeField] public Collider2D ColliderToDisable { get; private set; }
    }
}
