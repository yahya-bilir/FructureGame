using Characters;
using EventBusses;
using Events;
using UnityEngine;

public class MeleeAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;
    private readonly IEventBus _eventBus;
    private readonly float _damage;

    public MeleeAttacking(CharacterAnimationController animationController, float interval,
        CharacterCombatManager combatManager, IEventBus eventBus, float damage, GameObject model)
        : base(animationController, interval, combatManager, model)
    {
        _combatManager = combatManager;
        _eventBus = eventBus;
        _damage = damage;
    }

    protected override void OnAttack()
    {
        //_combatManager.TryDealMeleeDamage();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _eventBus.Subscribe<OnEnemyAttacked>(OnEnemyAttacked);
    }

    public override void OnExit()
    {
        base.OnExit();
        _eventBus.Unsubscribe<OnEnemyAttacked>(OnEnemyAttacked);
    }

    private void OnEnemyAttacked(OnEnemyAttacked attackedEnemy)
    {
        if(attackedEnemy.AttackedCharacter != _combatManager.Character) return;
        var lastFoundEnemy = _combatManager.LastFoundEnemy;
        if(lastFoundEnemy == null && lastFoundEnemy.IsCharacterDead) return;
        lastFoundEnemy.CharacterCombatManager.GetDamage(_damage);
    }
}