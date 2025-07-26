using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using Factions;
using IslandSystem;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters
{
    public class CharacterCombatManager : IDisposable
    {
        protected readonly CharacterPropertyManager CharacterPropertyManager;
        protected readonly CharacterVisualEffects CharacterVisualEffects;
        public readonly Character Character;
        protected IEventBus EventBus;
        private CancellationTokenSource _attackStateCts;
        private IslandManager _islandManager; public bool FleeingEnabled { get; private set; }
        public Vector3 FleePosition { get; private set; }
        public Character LastFoundEnemy { get; private set; }
        
        public CharacterCombatManager(CharacterPropertyManager characterPropertyManager, CharacterVisualEffects characterVisualEffects, Character character)
        {
            CharacterPropertyManager = characterPropertyManager;
            CharacterVisualEffects = characterVisualEffects;
            Character = character;
        }
        
        [Inject]
        private void Inject(IEventBus eventBus, IslandManager islandManager)
        {
            EventBus = eventBus;
            _islandManager = islandManager;
        }
        
        public virtual void GetDamage(float damage)
        {
            var damageData = CharacterPropertyManager.GetProperty(PropertyQuery.Health);
            var newHealth = damageData.TemporaryValue - damage;
            CharacterPropertyManager.SetPropertyTemporarily(PropertyQuery.Health, newHealth);
            //Debug.Log(newHealth);
            CharacterVisualEffects.OnCharacterTookDamage(newHealth, CharacterPropertyManager.GetProperty(PropertyQuery.MaxHealth).TemporaryValue);
            
            if(newHealth <= 0) OnCharacterDied().Forget();
        }
        
        protected virtual async UniTask OnCharacterDied()
        {
            await CharacterVisualEffects.OnCharacterDied();
            EventBus.Publish(new OnCharacterDied(Character));
        }
        
        private void OnEnemyBeingAttacked(OnEnemyBeingAttacked eventData)
        {
            //if(eventData.AttackedEnemy == _character) return;
            if(FleeingEnabled) return;
            if(Vector3.Distance(eventData.EnemyBeingAttackedPosition, eventData.AttackedEnemy.transform.position) > 5f) return;

            // calculate flee direction
            var currentPosition = eventData.AttackedEnemy.transform.position;
            var attackerPosition = eventData.EnemyBeingAttackedPosition;

            var directionAwayFromAttacker = (currentPosition - attackerPosition).normalized;
            var fleeTarget = currentPosition + directionAwayFromAttacker * 5; // örnek olarak 3 birim uzaklaş

            FleePosition = fleeTarget;
            //BeingAttacked().Forget();
        }

        private async UniTaskVoid BeingAttacked()
        {
            FleeingEnabled = true;
            _attackStateCts = new CancellationTokenSource();
            
            try
            {
                await UniTask.WaitForSeconds(3f, cancellationToken: _attackStateCts.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            
            FleeingEnabled = false;

        }

        private void DisableBeingAttacked()
        {
            _attackStateCts?.Cancel();
            FleeingEnabled = false;
        }

        public void Dispose()
        {
           // EventBus.Unsubscribe<OnEnemyBeingAttacked>(OnEnemyBeingAttacked);
        }
    }
}