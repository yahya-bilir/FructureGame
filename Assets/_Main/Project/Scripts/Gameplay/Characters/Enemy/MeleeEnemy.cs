using EventBusses;
using PropertySystem;
using VContainer;

namespace Characters.Enemy
{
    public class MeleeEnemy : EnemyBehaviour
    {
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override BaseAttacking CreateAttackingState()
        {
            return new MeleeAttacking(AnimationController, CharacterDataHolder.AttackingInterval,
                CharacterCombatManager, _eventBus,
                CharacterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue, model, CharacterMovementController, MainBase, this, AIText);
        }
    }
}