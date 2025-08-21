using PropertySystem;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyCombatManager : CharacterCombatManager
    {
        private readonly EnemyDestructionManager _enemyDestructionManager;

        public EnemyCombatManager(CharacterPropertyManager characterPropertyManager,
            CharacterVisualEffects characterVisualEffects, Character character,
            EnemyDestructionManager enemyDestructionManager) :
            base(characterPropertyManager, characterVisualEffects, character)
        {
            _enemyDestructionManager = enemyDestructionManager;
        }
        

        public override void GetDamage(float damage, DamageTypes damageType = DamageTypes.Normal,  GameObject damagedObject = null)
        {
            base.GetDamage(damage, damageType, damagedObject);
            _enemyDestructionManager.DestroyPartIfPossible(damagedObject);
        } 
    }
}