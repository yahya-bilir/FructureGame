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

        [Header("Spawn Points (Round-Robin)")]
        [SerializeField] private List<Transform> spawnPoints = new();

        public bool IsSpawningAvailable => SpawnedEnemies.Count < FactorySo.SpawnLimit;

        [field: SerializeField] public List<Character> SpawnedEnemies { get; private set; }

        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        // Round-robin için dahili index
        private int _nextSpawnIndex = 0;

        public void SpawnEnemy()
        {
            var random = Random.Range(0, FactorySo.SpawnableEnemies.Count);
            var enemyPrefab = FactorySo.SpawnableEnemies[random];

            // Listedeki bir sonraki noktayı al (liste boşsa Vector3.zero fallback)
            var spawnPos = GetNextSpawnPointPosition();

            var enemy = GameObject.Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(Faction.Enemy);

            // Not: İstersen burada da OnCharacterSpawned yayınlayabilirim;
            // şu an mevcut mimariyle ikinci overload yayınlıyor, bunu korudum.
            // _eventBus.Publish(new OnCharacterSpawned(enemy));
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
            _eventBus = eventBus;
        }

        public bool RemoveEnemyIfPossible(Character character)
        {
            return SpawnedEnemies.Contains(character) && SpawnedEnemies.Remove(character);
        }

        public void MultiplySpawnRate(float multiplier)
        {
            FactorySo.ChangeSpawnRate(FactorySo.SpawnRangeMax / multiplier);
        }

        private Vector3 GetNextSpawnPointPosition()
        {
            if (spawnPoints != null && spawnPoints.Count > 0)
            {
                if (_nextSpawnIndex >= spawnPoints.Count) _nextSpawnIndex = 0;
                var t = spawnPoints[_nextSpawnIndex];
                _nextSpawnIndex++;
                return t != null ? t.position : Vector3.zero;
            }

            // Liste boşsa sahne kökenine düşer (gerekirse özelleştirebilirsin)
            return Vector3.zero;
        }
    }
}
