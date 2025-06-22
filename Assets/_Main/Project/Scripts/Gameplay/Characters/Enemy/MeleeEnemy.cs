using EventBusses;
using PropertySystem;
using VContainer;

namespace Characters.Enemy
{
    public class MeleeEnemy : EnemyBehaviour
    {
        private IEventBus _eventBus;
        private AttackAnimationCaller _attackAnimationCaller;
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override void Start()
        {
            base.Start();
            _attackAnimationCaller = GetComponentInChildren<AttackAnimationCaller>();
            Resolver.Inject(_attackAnimationCaller);
        }

        protected override BaseAttacking CreateAttackingState()
        {
            return new MeleeAttacking(AnimationController, CharacterDataHolder.AttackingInterval, CharacterCombatManager, _eventBus, CharacterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
        }
    }
}