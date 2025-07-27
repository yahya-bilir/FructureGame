using System;
using System.Collections.Generic;
using Characters;
using EventBusses;
using Events;
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
        
        [field: SerializeField] public List<Character> SpawnedEnemies { get; private set; }

        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        public void SpawnEnemy()
        {
            var random = Random.Range(0, factorySo.SpawnableEnemies.Count);
            var enemyPrefab = factorySo.SpawnableEnemies[random];
            var enemy = GameObject.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn();
        }        
        
        public void SpawnEnemy(Character characterToSpawn, Vector2 spawnPosition)
        {
            var enemy = GameObject.Instantiate(characterToSpawn, spawnPosition, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn();
            _eventBus.Publish(new OnCharacterSpawned(enemy));
        }

        public void Initialize(IObjectResolver objectResolver, IEventBus eventBus)
        {
            _objectResolver = objectResolver;
            _eventBus =  eventBus;
        }

        public bool RemoveEnemyIfPossibe(Character character)
        {
            return SpawnedEnemies.Contains(character) && SpawnedEnemies.Remove(character);
        }

        public void ReplaceEnemy(Character newCharacter, Character oldCharacter)
        {
            _eventBus.Publish(new OnCharacterDied(oldCharacter));
            SpawnedEnemies.Add(newCharacter);
            _eventBus.Publish(new OnCharacterSpawned(newCharacter));

        }
    }
}