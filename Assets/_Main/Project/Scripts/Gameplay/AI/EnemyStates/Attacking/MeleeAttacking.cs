using Characters;
using Characters.Enemy;
using EventBusses;
using Events;
using UnityEngine;

public class MeleeAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;
    private readonly IEventBus _eventBus;
    private readonly float _damage;
    private readonly EnemyMovementController _enemyMovementController;

    public MeleeAttacking(CharacterAnimationController animationController, float interval,
        CharacterCombatManager combatManager, IEventBus eventBus, float damage, GameObject model,
        EnemyMovementController enemyMovementController)
        : base(animationController, interval, combatManager, model)
    {
        _combatManager = combatManager;
        _eventBus = eventBus;
        _damage = damage;
        _enemyMovementController = enemyMovementController;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _eventBus.Subscribe<OnEnemyAttacked>(OnEnemyAttacked);
        _enemyMovementController.StopCharacter(false);
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