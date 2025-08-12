using System;
using System.Collections.Generic;
using Characters;
using EventBusses;
using Events;
using Factions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Factories
{
    [Serializable]
    public class EnemyFactory
    {
        [field: SerializeField] public EnemyFactorySO FactorySo { get; private set; }
        [SerializeField] private Transform spawnPoint;
        public bool IsSpawningAvailable => SpawnedEnemies.Count < FactorySo.SpawnLimit;
        
        [field: SerializeField] public List<Character> SpawnedEnemies { get; private set; }

        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        public void SpawnEnemy()
        {
            var random = Random.Range(0, FactorySo.SpawnableEnemies.Count);
            var enemyPrefab = FactorySo.SpawnableEnemies[random];

            // X ve Z ekseninde ±1.5f rastgele kayma, Y sabit kalır
            var offset = new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f));
            var spawnPos = spawnPoint.position + offset;

            var enemy = GameObject.Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(Faction.Enemy);
        }
        public void SpawnEnemy(Character characterToSpawn, Vector2 spawnPosition)
        {
            var enemy = GameObject.Instantiate(characterToSpawn, spawnPosition, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(Faction.Enemy);
            _eventBus.Publish(new OnCharacterSpawned(enemy));
        }

        public void Initialize(IObjectResolver objectResolver, IEventBus eventBus)
        {
            _objectResolver = objectResolver;
            _eventBus =  eventBus;
        }

        public bool RemoveEnemyIfPossible(Character character)
        {
            return SpawnedEnemies.Contains(character) && SpawnedEnemies.Remove(character);
        }

        public void MultiplySpawnRate(float multiplier)
        {
            FactorySo.ChangeSpawnRate(FactorySo.SpawnRangeMax / multiplier);
        }
    }
}