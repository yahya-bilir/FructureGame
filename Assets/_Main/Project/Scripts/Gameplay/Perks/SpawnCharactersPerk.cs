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
        
        private EnemyManager _enemyManager;

        [Inject]
        private void Inject(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }
        
        public override void OnDragEndedOnScene(Vector2 worldPos, float radius)
        {
            var spawnCount = Random.Range(min, max);
            SpawnCharacters(worldPos, spawnCount, radius).Forget();

        }

        private async UniTask SpawnCharacters(Vector2 worldPos, int count, float radius)
        {
            for (int i = 0; i < count; i++)
            {
                var spawnPosition = worldPos + Random.insideUnitCircle * radius;
                _enemyManager.SpawnPlayerArmyCharacter(characterToSpawn, spawnPosition);
                Debug.Log($"Spawned {characterToSpawn.name} at {spawnPosition}");
                await UniTask.WaitForSeconds(0.05f);
            }
        }
    }
}