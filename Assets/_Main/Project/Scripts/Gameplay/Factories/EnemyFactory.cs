using System;
using System.Collections.Generic;
using Characters;
using Characters.Enemy;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Factories
{
    [Serializable]
    public class EnemyFactory
    {
        [SerializeField] private EnemyFactorySO factorySo;
        [SerializeField] private Transform spawnPoint;
        
        private List<EnemyBehaviour> _spawnedEnemies = new();
        public bool IsSpawningAvailable => _spawnedEnemies.Count < factorySo.SpawnLimit;
        public float SpawnInterval => factorySo.SpawnInterval;
        public float InitialSpawnInterval => factorySo.InitialSpawnInterval;

        private IObjectResolver _objectResolver;
        public void SpawnEnemy()
        {
            var random = Random.Range(0, factorySo.SpawnableEnemies.Count);
            var enemyPrefab = factorySo.SpawnableEnemies[random];
            var enemy = GameObject.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            _spawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(factorySo.Faction);
        }

        public void Initialize(IObjectResolver objectResolver) => _objectResolver = objectResolver;
    }
}