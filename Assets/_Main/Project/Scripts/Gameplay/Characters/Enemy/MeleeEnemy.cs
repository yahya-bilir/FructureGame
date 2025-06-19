using AI.Base.Interfaces;
using AI.EnemyStates;

namespace Characters.Enemy
{
    public class MeleeEnemy : EnemyBehaviour
    {
        protected override IState CreateAttackingState()
        {
            return new MeleeAttacking(AnimationController, CharacterDataHolder.AttackingInterval);
        }
    }
}