using Characters;
using EventBusses;
using Events;
using Events.IslandEvents;
using IslandSystem;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyFactory PlayerArmyFactory { get; private set; }
        private IObjectResolver _resolver;
        private IEventBus _eventBus;
        private IslandManager _islandManager;

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus, IslandManager islandManager)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            _islandManager = islandManager;
        }

        private void Awake()
        {
            foreach (var chr in FindObjectsOfType<Character>())
            {
                _resolver.Inject(chr);
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Subscribe<OnIslandStarted>(OnIslandStarted);
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Subscribe<OnCharacterUpgraded>(OnCharacterUpgraded);

        }

        private void OnCharacterUpgraded(OnCharacterUpgraded eventData)
        {
            PlayerArmyFactory.ReplaceEnemy(eventData.AddedCharacter, eventData.DestroyedCharacter);
        }

        private void OnCharacterDied(OnCharacterDied eventData)
        {
            PlayerArmyFactory.RemoveEnemyIfPossibe(eventData.Character);
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            foreach (var chr in PlayerArmyFactory.SpawnedEnemies)
            {
                chr.CharacterIslandController.SetNextIsland(eventData.SelectedIsland);
            }
        }        
        
        private void OnIslandStarted(OnIslandStarted eventData)
        {
            foreach (var chr in PlayerArmyFactory.SpawnedEnemies)
            {
                chr.CharacterIslandController.SetPreviousIsland(eventData.StartedIsland);
            }
        }

        private void Start()
        {
            PlayerArmyFactory.Initialize(_resolver, _islandManager, _eventBus);
        }

        public void SpawnPlayerArmyCharacter(Character character, Vector2 position)
        {
            PlayerArmyFactory.SpawnEnemy(character, position);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Unsubscribe<OnIslandStarted>(OnIslandStarted);
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Unsubscribe<OnCharacterUpgraded>(OnCharacterUpgraded);
        }
    }
}