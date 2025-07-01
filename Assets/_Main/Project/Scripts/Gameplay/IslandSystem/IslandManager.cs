using System.Collections.Generic;
using EventBusses;
using Events;
using Events.IslandEvents;
using UnityEngine;
using VContainer;

namespace IslandSystem
{
    public class IslandManager : MonoBehaviour
    {
        [SerializeField] private Island firstIsland;
        [SerializeField] private List<Island> allIslands;
        public bool FightCanStart { get; private set; }

        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;
        private AstarPath _astarPath;

        [Inject]
        private void Inject(IObjectResolver objectResolver, IEventBus eventBus, AstarPath aStarPath)
        {
            _objectResolver = objectResolver;
            _eventBus = eventBus;
            _astarPath = aStarPath;
        }
        private void Awake()
        {
            _objectResolver.Inject(firstIsland);
            foreach (var island in allIslands)
            {
                _objectResolver.Inject(island);
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnIslandStarted>(OnIslandStarted);
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
        }

        private void Start()
        {
            firstIsland.StartIslandOpeningActions();
        }

        private void OnIslandStarted(OnIslandStarted eventData)
        {
            _astarPath.Scan();
            FightCanStart = true;
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            FightCanStart = false;
        }

        private void OnCharacterDied(OnCharacterDied data)
        {
            _astarPath.Scan();
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandStarted>(OnIslandStarted);
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
        }
    }
}