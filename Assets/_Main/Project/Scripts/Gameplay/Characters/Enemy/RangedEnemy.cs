using WeaponSystem.Managers;

namespace Characters.Enemy
{
    public class RangedEnemy : EnemyBehaviour
    {
        protected override BaseAttacking CreateAttackingState()
        {
            return new RangedAttacking(AnimationController, CharacterDataHolder.AttackingInterval,
                CharacterCombatManager, _eventBus, CharacterWeaponManager);
            
        }
    }
}