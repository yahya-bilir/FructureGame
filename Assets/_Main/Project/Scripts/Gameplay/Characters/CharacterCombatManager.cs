using System.Linq;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters
{
    public class CharacterCombatManager
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterVisualEffects _characterVisualEffects;
        private readonly Character _character;
        private IEventBus _eventBus;

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
        }
        
        public void GetDamage(float damage)
        {
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Health);
            var newHealth = damageData.TemporaryValue - damage;
            _characterPropertyManager.SetProperty(PropertyQuery.Health, newHealth);
            Debug.Log(newHealth);
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
        
    }
}