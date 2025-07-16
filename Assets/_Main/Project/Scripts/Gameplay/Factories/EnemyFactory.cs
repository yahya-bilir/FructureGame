using System;
using System.Collections.Generic;
using Characters;
using Characters.Enemy;
using EventBusses;
using Events;
using IslandSystem;
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
        private IslandManager _islandManager;
        private IEventBus _eventBus;

        public void SpawnEnemy()
        {
            var random = Random.Range(0, factorySo.SpawnableEnemies.Count);
            var enemyPrefab = factorySo.SpawnableEnemies[random];
            var enemy = GameObject.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(factorySo.Faction);
        }        
        
        public void SpawnEnemy(Character characterToSpawn, Vector2 spawnPosition)
        {
            var enemy = GameObject.Instantiate(characterToSpawn, spawnPosition, Quaternion.identity);
            _objectResolver.InjectGameObject(enemy.gameObject);
            SpawnedEnemies.Add(enemy);
            enemy.InitializeOnSpawn(factorySo.Faction);
            enemy.CharacterIslandController.SetPreviousIsland(_islandManager.CurrentIsland);
            enemy.CharacterIslandController.SetNextIsland(_islandManager.CurrentIsland);
            _eventBus.Publish(new OnCharacterSpawned(enemy, _islandManager.firstIsland));
        }

        public void Initialize(IObjectResolver objectResolver, IslandManager islandManager, IEventBus eventBus)
        {
            _objectResolver = objectResolver;
            _islandManager = islandManager;
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
            newCharacter.CharacterIslandController.SetPreviousIsland(_islandManager.CurrentIsland);
            newCharacter.CharacterIslandController.SetNextIsland(_islandManager.CurrentIsland);
            _eventBus.Publish(new OnCharacterSpawned(newCharacter, _islandManager.firstIsland));

        }
    }
}