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
        [SerializeField] private bool isFirstIsland;
        
        [SerializeField] private List<Island> nextIslandsToBeAvailable;
        [SerializeField] private List<GameObject> collidersToDisableWhenSelected;
        [SerializeField] private List<OpeningSection> openingSections;
        [SerializeField] private Transform mainCameraPosition;
        [SerializeField] private Transform cardSelectionCameraPosition;
        [SerializeField] private Transform openingCameraPosition;
        
        
        
        [SerializeField] private Transform formationAnchor;
        [SerializeField] private Collider2D nextIslandJumpingPos;
        [SerializeField] private Collider2D placingPosCollider;
        [SerializeField] private List<IslandCloud> islandClouds;
        
        private IslandOpeningSystem _islandOpeningSystem;
        private CloudMovementManager _cloudManager;
        private IslandCharactersController _islandCharactersController;
        private IslandCameraMovementManager _islandCameraMovementManager;
        private IslandManager _islandManager;
        private AstarPath _astarPath;
        public IslandJumpingActions JumpingActions { get; private set; }

        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus, IObjectResolver resolver, CloudMovementManager cloudManager, IslandManager islandManager, AstarPath astarPath)
        {
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _resolver = resolver;
            _cloudManager = cloudManager;
            _islandManager = islandManager;
            _astarPath = astarPath;
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
            _islandCharactersController = new IslandCharactersController(openingSections, _eventBus, this, isFirstIsland);
            _islandCameraMovementManager = new IslandCameraMovementManager(mainCameraPosition, _camerasManager, _eventBus, this, cardSelectionCameraPosition, _islandManager, openingCameraPosition);
            _islandOpeningSystem = new IslandOpeningSystem(
                _islandCameraMovementManager, _rateChanger,
                _scaler, _eventBus, this, collidersToDisableWhenSelected, JumpingActions, _cloudManager, _islandCharactersController, islandClouds, _islandManager, _astarPath);


            _islandOpeningSystem.Initialize();

            _resolver.Inject(_islandOpeningUI);
            _resolver.Inject(JumpingActions);
            _islandCharactersController.Initialize();
            _islandCameraMovementManager.Initialize();
            _islandOpeningUI.Initialize(this);

            JumpingActions.CacheJumpArea();
            _resolver.Inject(_scaler);
            
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
