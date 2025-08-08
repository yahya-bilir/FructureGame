using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;

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
        }

        private void Start()
        {
            foreach (var enemyFactory in enemyFactories)
            {
                enemyFactory.Initialize(_resolver, _eventBus);
                SpawnFactoryEnemies(enemyFactory).Forget();
            }
        }

        private async UniTask SpawnFactoryEnemies(EnemyFactory factory)
        {
            await UniTask.WaitForSeconds(factory.FactorySo.InitialSpawnInterval);
    
            if (factory.IsSpawningAvailable)
            {
                factory.SpawnEnemy();
            }

            while (factory.IsSpawningAvailable)
            {
                await UniTask.WaitForSeconds(Random.Range(factory.FactorySo.SpawnRangeMin, factory.FactorySo.SpawnRangeMax));
                factory.SpawnEnemy();
            }
        }

        private void OnCharacterDied(OnCharacterDied eventData)
        {
            foreach (var factory in enemyFactories)
            {
                factory.RemoveEnemyIfPossible(eventData.Character);
            }
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
        }
    }
}