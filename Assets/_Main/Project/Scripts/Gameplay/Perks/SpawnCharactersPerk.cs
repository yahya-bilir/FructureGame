using Characters;
using Factories;
using Perks.Base;
using UnityEngine;
using VContainer;

namespace Perks
{
    [CreateAssetMenu(fileName = "UpgradeCharactersPerk", menuName = "Scriptable Objects/Perks/Spawn Characters Perk")]
    public class SpawnCharactersPerk : ClickableActionSo
    {
        [SerializeField] private Character characterToSpawn;
        
        [Header("Spawn Range")]
        [SerializeField] private int min;
        [SerializeField] private int max;
        
        private EnemyFactoryManager _enemyFactoryManager;
        
        [Inject]
        private void Inject(EnemyFactoryManager enemyFactoryManager)
        {
            _enemyFactoryManager = enemyFactoryManager;
        }
        
        public override void OnDragEndedOnScene(Vector2 worldPos)
        {
            var spawnCount = Random.Range(min, max);
            for (int i = 0; i < spawnCount; i++)
            {
                var spawnPosition = worldPos + Random.insideUnitCircle * 2f;
                _enemyFactoryManager.SpawnEnemy(characterToSpawn, spawnPosition);
                Debug.Log($"Spawned {characterToSpawn.name} at {spawnPosition}");
            }
        }
    }
}