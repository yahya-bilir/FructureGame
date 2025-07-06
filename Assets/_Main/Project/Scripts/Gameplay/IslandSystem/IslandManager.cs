using System.Collections;
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
        
        public Island CurrentIsland { get; private set; }

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
            _eventBus.Subscribe<OnAllPerksSelected>(OnAllPerksSelected);
        }
        

        private IEnumerator Start()
        {
            firstIsland.StartIslandOpeningActions();
            yield return new WaitForSeconds(1f);
            _eventBus.Publish(new OnAllIslandEnemiesKilled(firstIsland));
        }

        private void OnIslandStarted(OnIslandStarted eventData)
        {
            _astarPath.Scan();
            FightCanStart = true;
            CurrentIsland = eventData.StartedIsland;
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            FightCanStart = false;
        }

        private void OnCharacterDied(OnCharacterDied data)
        {
            _astarPath.Scan();
        }
        
        private void OnAllPerksSelected(OnAllPerksSelected eventData)
        {
            _eventBus.Publish(new OnIslandFinished(CurrentIsland));
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandStarted>(OnIslandStarted);
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Unsubscribe<OnAllPerksSelected>(OnAllPerksSelected);

        }
    }
}