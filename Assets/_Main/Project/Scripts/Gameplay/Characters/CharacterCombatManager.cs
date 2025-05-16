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
        private ShineEffect _shineEffect;

        public CharacterCombatManager(Character connectedCharacter, CharacterPropertyManager characterPropertyManager,
            CharacterDataHolder characterDataHolder, ShineEffect shineEffect)
        {
            _connectedCharacter = connectedCharacter;
            _characterPropertyManager = characterPropertyManager;
            _characterDataHolder = characterDataHolder;
            _shineEffect = shineEffect;
        }
        
        public void GetDamage(float damage)
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