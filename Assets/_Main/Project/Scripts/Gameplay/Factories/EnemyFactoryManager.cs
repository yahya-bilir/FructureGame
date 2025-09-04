using System;
using System.Collections.Generic;
using Characters;
using Characters.StationaryGunHolders;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;

        private IObjectResolver _resolver;
        private IEventBus _eventBus;
        private GunHolderPlacer _gunHolderPlacer;

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus, GunHolderPlacer gunHolderPlacer)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            _gunHolderPlacer = gunHolderPlacer;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                foreach (var factory in enemyFactories)
                {
                    //factory.MultiplySpawnRate(2);
                }
            }
        }

        private async UniTask SpawnFactoryEnemies(EnemyFactory factory)
        {
            await UniTask.WaitForSeconds(factory.FactorySo.InitialSpawnInterval);
    
            while (factory.IsSpawningAvailable)
            {
                if (!_gunHolderPlacer.IsThereAnyWeapon)
                {
                    await UniTask.Yield();
                    continue;
                }

                factory.SpawnEnemy();
                await UniTask.WaitForSeconds(Random.Range(factory.FactorySo.SpawnRangeMin, factory.FactorySo.SpawnRangeMax));
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