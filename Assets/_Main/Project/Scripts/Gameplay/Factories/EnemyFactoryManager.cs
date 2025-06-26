using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
        private IObjectResolver _resolver;
        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
            var characters = FindObjectsOfType<Character>();
            foreach (var chr in characters)
            {
                _resolver.Inject(chr);
            }
        }
        
        private void Start()
        {
            foreach (var enemyFactory in enemyFactories)
            {
                enemyFactory.Initialize(_resolver);
                SpawnFactoryEnemies(enemyFactory).Forget();
            }
        }

        private async UniTask SpawnFactoryEnemies(EnemyFactory factory)
        {
            await UniTask.WaitForSeconds(factory.InitialSpawnInterval);
    
            if (factory.IsSpawningAvailable)
            {
                factory.SpawnEnemy();
            }

            while (factory.IsSpawningAvailable)
            {
                await UniTask.WaitForSeconds(factory.SpawnInterval);
                factory.SpawnEnemy();
            }
        }

        public void SpawnEnemy(Character character, Vector2 position)
        {
            
        }
    }
}