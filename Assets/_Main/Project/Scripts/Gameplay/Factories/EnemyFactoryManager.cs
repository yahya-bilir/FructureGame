using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
        private Transform _playerTransform;
        [Inject]
        private void Inject(PlayerJoystickMovement joystickMovement)
        {
            _playerTransform = joystickMovement.transform;
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