using System.Linq;
using _Main.Project.Scripts.Utils;
using PropertySystem;
using UnityEngine;

namespace Characters
{
    public class CharacterCombatManager
    {
        private Character _connectedCharacter;
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterDataHolder _characterDataHolder;
        private float _passedTimeSinceLastAttack;
        private ShineEffect _shineEffect;

        public CharacterCombatManager(Character connectedCharacter, CharacterPropertyManager characterPropertyManager,
            CharacterDataHolder characterDataHolder)
        {
            _connectedCharacter = connectedCharacter;
            _characterPropertyManager = characterPropertyManager;
            _characterDataHolder = characterDataHolder;
            _shineEffect = new ShineEffect(_connectedCharacter.GetComponentsInChildren<SpriteRenderer>().ToList(), _characterDataHolder.ShineColor, _characterDataHolder.ShineDuration);
        }

        public virtual void OnGettingAttacked(float damage, float attackInterval)
        {
            if (_passedTimeSinceLastAttack < attackInterval)
            {
                _passedTimeSinceLastAttack += Time.deltaTime;
                return;
            }

            _passedTimeSinceLastAttack = 0f;
            GetDamage(damage);
        }
        
        protected virtual void GetDamage(float damage)
        {
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Health);
            var newHealth = damageData.TemporaryValue - damage;
            _characterPropertyManager.SetProperty(PropertyQuery.Health, newHealth);
            _shineEffect.Shine();
            
            if(newHealth <= 0) CharacterIsDead();
        }

        private void CharacterIsDead()
        {
            
        }
    }
}