using System;
using System.Collections.Generic;
using CommonComponents;
using Cysharp.Threading.Tasks;
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

        [SerializeField] private List<Island> nextIslandsToBeAvailable;
        [SerializeField] private List<GameObject> collidersToDisableWhenSelected;
        [SerializeField] private List<OpeningSection> openingSections;
        [SerializeField] private Transform mainCameraPosition;
        [SerializeField] private Transform cardSelectionCameraPosition;

        [SerializeField] private Transform formationAnchor;
        [SerializeField] private Collider2D nextIslandJumpingPos;
        [SerializeField] private Collider2D placingPosCollider;

        private IslandOpeningSystem _islandOpeningSystem;
        private CloudMovementManager _cloudManager;
        private IslandCharactersController _islandCharactersController;
        private IslandCameraMovementManager _islandCameraMovementManager;
        private IslandManager _islandManager;
        public IslandJumpingActions JumpingActions { get; private set; }

        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus, IObjectResolver resolver, CloudMovementManager cloudManager, IslandManager islandManager)
        {
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _resolver = resolver;
            _cloudManager = cloudManager;
            _islandManager = islandManager;
            _eventBus.Subscribe<OnIslandFinished>(OnIslandFinished);

        }

        private void Awake()
        {
            _scaler = GetComponentInChildren<Scaler>();
            _rateChanger = GetComponentInChildren<RateChanger>();
            _islandOpeningUI = GetComponentInChildren<IslandOpeningUI>();

        }
        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandFinished>(OnIslandFinished);
        }

        private void Start()
        {
            JumpingActions = new IslandJumpingActions(nextIslandJumpingPos, formationAnchor, this, placingPosCollider);
            _islandCharactersController = new IslandCharactersController(openingSections, _eventBus, this);
            _islandCameraMovementManager = new IslandCameraMovementManager(mainCameraPosition, _camerasManager, _eventBus, this, cardSelectionCameraPosition, _islandManager);
            _islandOpeningSystem = new IslandOpeningSystem(
                _islandCameraMovementManager, _rateChanger,
                _scaler, _eventBus, this, collidersToDisableWhenSelected, JumpingActions, _cloudManager, _islandCharactersController);


            _islandOpeningSystem.Initialize();

            _resolver.Inject(_islandOpeningUI);
            _resolver.Inject(JumpingActions);
            _islandCharactersController.Initialize();
            _islandCameraMovementManager.Initialize();
            _islandOpeningUI.Initialize(this);

            JumpingActions.CacheJumpArea();
        }

        [Button]
        private void OnIslandFinished(OnIslandFinished eventData)
        {
            if(eventData.FinishedIsland != this) return;
            foreach (var nextIsland in nextIslandsToBeAvailable)
            {
                nextIsland.MakeIslandAvailable();
            }
        }

        private void MakeIslandAvailable()
        {
            _islandOpeningUI.MakeIslandAvailable();
        }

        [Button]
        public void StartIslandOpeningActions()
        {
            _islandOpeningUI.StartIslandOpeningActions().Forget();
        }
    }
}
