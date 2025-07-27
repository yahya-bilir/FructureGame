using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
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
        public Vector3 FleePosition { get; private set; }
        public Character LastFoundEnemy { get; private set; }
        
        public CharacterCombatManager(CharacterPropertyManager characterPropertyManager, CharacterVisualEffects characterVisualEffects, Character character)
        {
            CharacterPropertyManager = characterPropertyManager;
            CharacterVisualEffects = characterVisualEffects;
            Character = character;
        }
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            EventBus = eventBus;
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
        
        public Character FindNearestEnemy()
        {
            //var range = CharacterPropertyManager.GetProperty(PropertyQuery.AttackRange).TemporaryValue;
            var origin = Character.transform.position;

            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, 50, LayerMask.GetMask("AI"));

            if (hits.Length == 0) return null;

            var nearest = hits
                .Select(c => c.GetComponent<Character>())
                .Where(c => c != null && c.Faction != Character.Faction && !c.IsCharacterDead) // Karakterin kendi faction'ı dışındakiler
                .OrderBy(c => Vector2.Distance(origin, c.transform.position))
                .FirstOrDefault();

            LastFoundEnemy = nearest;
            return nearest;
        }
        
        protected virtual async UniTask OnCharacterDied()
        {
            await CharacterVisualEffects.OnCharacterDied();
            EventBus.Publish(new OnCharacterDied(Character));
        }
        
        private void OnEnemyBeingAttacked(OnEnemyBeingAttacked eventData)
        {
            //if(eventData.AttackedEnemy == _character) return;
            if(Vector3.Distance(eventData.EnemyBeingAttackedPosition, eventData.AttackedEnemy.transform.position) > 5f) return;

            // calculate flee direction
            var currentPosition = eventData.AttackedEnemy.transform.position;
            var attackerPosition = eventData.EnemyBeingAttackedPosition;

            var directionAwayFromAttacker = (currentPosition - attackerPosition).normalized;
            var fleeTarget = currentPosition + directionAwayFromAttacker * 5; // örnek olarak 3 birim uzaklaş

            FleePosition = fleeTarget;
            //BeingAttacked().Forget();
        }
        

        public void Dispose()
        {
           // EventBus.Unsubscribe<OnEnemyBeingAttacked>(OnEnemyBeingAttacked);
        }
    }
}