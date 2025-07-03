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
        [SerializeField] private Transform cameraPositioner;

        [SerializeField] private Transform formationAnchor;
        [SerializeField] private Collider2D nextIslandJumpingPos;
        [SerializeField] private Collider2D placingPosCollider;

        private IslandOpeningSystem _islandOpeningSystem;
        private CloudMovementManager _cloudManager;
        public IslandJumpingActions JumpingActions { get; private set; }

        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus, IObjectResolver resolver, CloudMovementManager cloudManager)
        {
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _resolver = resolver;
            _cloudManager = cloudManager;

        }

        private void Awake()
        {
            _scaler = GetComponentInChildren<Scaler>();
            _rateChanger = GetComponentInChildren<RateChanger>();
            _islandOpeningUI = GetComponentInChildren<IslandOpeningUI>();

        }

        private void Start()
        {
            JumpingActions = new IslandJumpingActions(nextIslandJumpingPos, formationAnchor, this, placingPosCollider);
            _islandOpeningSystem = new IslandOpeningSystem(
                _camerasManager, _rateChanger,
                openingSections, _scaler, cameraPositioner,
                _eventBus, this, collidersToDisableWhenSelected, JumpingActions, _cloudManager);

            _islandOpeningSystem.Initialize();

            _resolver.Inject(_islandOpeningUI);
            _resolver.Inject(JumpingActions);
            _islandOpeningUI.Initialize(this);

            JumpingActions.CacheJumpArea();
        }

        [Button]
        private void OnIslandFinished()
        {
            _eventBus.Publish(new OnIslandFinished(this));

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
