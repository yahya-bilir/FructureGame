using Characters;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyFactory PlayerArmyFactory { get; private set; }
        private IObjectResolver _resolver;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus)
        {
            _resolver = resolver;
            _eventBus = eventBus;
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
        
        private void Start()
        {
            PlayerArmyFactory.Initialize(_resolver, _eventBus);
        }

        public void SpawnPlayerArmyCharacter(Character character, Vector2 position)
        {
            PlayerArmyFactory.SpawnEnemy(character, position);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Unsubscribe<OnCharacterUpgraded>(OnCharacterUpgraded);
        }
    }
}