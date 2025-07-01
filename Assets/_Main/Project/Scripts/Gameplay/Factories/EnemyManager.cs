using System.Collections.Generic;
using Characters;
using EventBusses;
using Events.IslandEvents;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyFactory PlayerArmyFactory { get; private set; }
        private IObjectResolver _resolver;
        private List<Character> _allCharacters;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus)
        {
            _resolver = resolver;
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _allCharacters = new List<Character>(FindObjectsOfType<Character>());
            foreach (var chr in _allCharacters)
            {
                _resolver.Inject(chr);
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Subscribe<OnIslandStarted>(OnIslandStarted);

        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            foreach (var chr in _allCharacters)
            {
                chr.CharacterIslandController.SetNextIsland(eventData.SelectedIsland);
            }
        }        
        
        private void OnIslandStarted(OnIslandStarted eventData)
        {
            foreach (var chr in _allCharacters)
            {
                chr.CharacterIslandController.SetPreviousIsland(eventData.StartedIsland);
            }
        }

        private void Start()
        {
            PlayerArmyFactory.Initialize(_resolver);
        }

        public void SpawnPlayerArmyCharacter(Character character, Vector2 position)
        {
            PlayerArmyFactory.SpawnEnemy(character, position);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
            _eventBus.Unsubscribe<OnIslandStarted>(OnIslandStarted);

        }
    }
}