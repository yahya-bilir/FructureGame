using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
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
                
                factory.SpawnEnemy();
            }
        }
    }
}