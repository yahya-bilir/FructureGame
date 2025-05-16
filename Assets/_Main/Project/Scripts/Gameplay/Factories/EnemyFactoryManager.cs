using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSave.Runtime;
using Player;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
        private Transform _playerTransform;
        private GameData _gameData;
        
        [Inject]
        private void Inject(PlayerController playerController, GameData gameData)
        {
            _playerTransform = playerController.transform;
            _gameData = gameData;
        }
        private void Start()
        {
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
                
                factory.SpawnEnemy(_playerTransform);
            }
        }
    }
}