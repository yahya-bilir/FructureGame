using Characters;
using Cysharp.Threading.Tasks;
using Factories;
using Perks.Base;
using UnityEngine;
using VContainer;

namespace Perks
{
    [CreateAssetMenu(fileName = "SpawnCharactersPerk", menuName = "Scriptable Objects/Perks/Spawn Characters Perk")]
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
            SpawnCharacters(worldPos, spawnCount).Forget();

        }

        private async UniTask SpawnCharacters(Vector2 worldPos, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var spawnPosition = worldPos + Random.insideUnitCircle * 1f;
                _enemyFactoryManager.SpawnPlayerArmyCharacter(characterToSpawn, spawnPosition);
                Debug.Log($"Spawned {characterToSpawn.name} at {spawnPosition}");
                await UniTask.WaitForSeconds(0.05f);
            }
        }
    }
}