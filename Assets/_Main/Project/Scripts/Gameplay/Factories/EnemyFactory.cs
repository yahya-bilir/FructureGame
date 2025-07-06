using System;
using System.Collections.Generic;
using Characters;
using Characters.Enemy;
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
        }

        public void Initialize(IObjectResolver objectResolver, IslandManager islandManager)
        {
            _objectResolver = objectResolver;
            _islandManager = islandManager;
        }

        public bool RemoveEnemyIfPossibe(Character character)
        {
            return SpawnedEnemies.Contains(character) && SpawnedEnemies.Remove(character);
        }

        public void ReplaceEnemy(Character newCharacter, Character oldCharacter)
        {
            if(!RemoveEnemyIfPossibe(oldCharacter)) return;
            SpawnedEnemies.Add(newCharacter);
            newCharacter.CharacterIslandController.SetPreviousIsland(_islandManager.CurrentIsland);
            newCharacter.CharacterIslandController.SetNextIsland(_islandManager.CurrentIsland);

        }
    }
}