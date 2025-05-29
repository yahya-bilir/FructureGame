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
        protected readonly CharacterPropertyManager _characterPropertyManager;
        protected readonly CharacterVisualEffects _characterVisualEffects;
        protected readonly Character _character;
        private IEventBus _eventBus;
        private CancellationTokenSource _attackStateCts;
        public bool FleeingEnabled { get; private set; }
        public Vector3 FleePosition { get; private set; }
        public CharacterCombatManager(CharacterPropertyManager characterPropertyManager, CharacterVisualEffects characterVisualEffects, Character character)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterVisualEffects = characterVisualEffects;
            _character = character;
        }
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnEnemyBeingAttacked>(OnEnemyBeingAttacked);

        }
        
        public virtual void GetDamage(float damage)
        {
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Health);
            var newHealth = damageData.TemporaryValue - damage;
            _characterPropertyManager.SetProperty(PropertyQuery.Health, newHealth);
            //Debug.Log(newHealth);
            _characterVisualEffects.OnCharacterTookDamage(newHealth, _characterPropertyManager.GetProperty(PropertyQuery.MaxHealth).TemporaryValue);
            
            if(newHealth <= 0) OnCharacterDied().Forget();
        }
        
        private async UniTask OnCharacterDied()
        {
            await _characterVisualEffects.OnCharacterDied();
            _eventBus.Publish(new OnCharacterDiedEvent(_character));
        }

        public Character FindNearestEnemy()
        {
            var range = _characterPropertyManager.GetProperty(PropertyQuery.AttackRange).TemporaryValue;
            var origin = _character.transform.position;

            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, LayerMask.GetMask("Enemy"));

            if (hits.Length == 0)
                return null;

            var nearest = hits
                .OrderBy(c => Vector2.Distance(origin, c.transform.position))
                .FirstOrDefault();

            if (nearest == null) return null;
            var component = nearest.GetComponent<Character>();
            _eventBus.Publish(new OnNearbyEnemyFoundEvent(component));
            return component;
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
            BeingAttacked().Forget();
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
            _eventBus.Unsubscribe<OnEnemyBeingAttacked>(OnEnemyBeingAttacked);
        }
    }
}