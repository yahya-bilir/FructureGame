using System;
using System.Collections.Generic;
using Characters.Enemy;
using DataSave.Runtime;
using PropertySystem;
using UnityEngine;
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

        public void SpawnEnemy(Transform playerTransform)
        {
            var random = Random.Range(0, factorySo.SpawnableEnemies.Count);
            var enemyPrefab = factorySo.SpawnableEnemies[random];
            var enemy = GameObject.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(playerTransform);
        }
    }
}