using System;
using System.Collections.Generic;
using Characters;
using Characters.Player;
using Cysharp.Threading.Tasks;
using DataSave.Runtime;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
        private PlayerController _playerController;
        private GameData _gameData;
        private CharacterCombatManager _playerCombatManager;
        
        [Inject]
        private void Inject(PlayerController playerController, GameData gameData)
        {
            _playerController = playerController;
            _gameData = gameData;
        }
        private void Start()
        {
            _playerCombatManager = _playerController.CharacterCombatManager;

            foreach (var enemyFactory in enemyFactories)
            {
                SpawnFactoryEnemies(enemyFactory).Forget();
            }
        }

        private async UniTask SpawnFactoryEnemies(EnemyFactory factory)
        {
            while (factory.IsSpawningAvailable)
            {
                await UniTask.WaitForSeconds(factory.SpawnInterval);
                
                factory.SpawnEnemy(_playerController.transform, _playerCombatManager);
            }
        }
    }
}