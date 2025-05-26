using System.Collections.Generic;
using Characters;
using Characters.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class EnemyFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyFactory> enemyFactories;
        private PlayerController _playerController;
        private CharacterCombatManager _playerCombatManager;
        private IObjectResolver _resolver;
        [Inject]
        private void Inject(PlayerController playerController, IObjectResolver resolver)
        {
            _playerController = playerController;
            _resolver = resolver;
        }

        private void Start()
        {
            _playerCombatManager = _playerController.CharacterCombatManager;

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
                factory.SpawnEnemy(_playerCombatManager);
            }

            while (factory.IsSpawningAvailable)
            {
                await UniTask.WaitForSeconds(factory.SpawnInterval);
                factory.SpawnEnemy(_playerCombatManager);
            }
        }
        
        
    }
}