using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using EventBusses;
using Factories;
using Perks.Base;
using UnityEngine;
using VContainer;

namespace Perks
{
    [CreateAssetMenu(fileName = "MultiplyCharactersPerk", menuName = "Scriptable Objects/Perks/Multiply Characters Perk")]

    public class MultiplyCharactersPerk : ClickableActionSo
    {
        private EnemyManager _enemyManager;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(EnemyManager enemyManager, IEventBus eventBus)
        {
            _enemyManager = enemyManager;
            _eventBus = eventBus;
        }
        
        public override void OnDrag(Vector2 worldPos, float radius)
        {
            base.OnDrag(worldPos, radius);
            SelectCharacters();
            DeselectCharacters();
        }

        public void OnSelected()
        {
            
        }

        public override void OnDragEndedOnScene(Vector2 worldPos, float radius)
        {
            CopyCharacters(worldPos, Characters, radius).Forget();
        }

        private async UniTask CopyCharacters(Vector2 worldPos, List<Character> characters, float radius)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                var characterToSpawn = characters[i];
                var spawnPosition = worldPos + Random.insideUnitCircle * radius;
                _enemyManager.SpawnPlayerArmyCharacter(characterToSpawn, spawnPosition);
                Debug.Log($"Spawned {characterToSpawn.name} at {spawnPosition}");
                await UniTask.WaitForSeconds(0.05f);
            }
        }
    }
}