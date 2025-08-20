using Characters;
using Characters.BaseSystem;
using Characters.Enemy;
using EventBusses;
using Events;
using TMPro;
using UnityEngine;

public class MeleeAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;
    private readonly IEventBus _eventBus;
    private readonly float _damage;
    private readonly EnemyMovementController _enemyMovementController;
    private readonly MainBase _mainBase;
    private readonly MeleeEnemy _meleeEnemy;
    private readonly TextMeshPro _aıText;
    private Quaternion _targetRotation;
    private float _rotationSpeed = 5f;
    
    public MeleeAttacking(CharacterAnimationController animationController, float interval,
        CharacterCombatManager combatManager, IEventBus eventBus, float damage, GameObject model,
        EnemyMovementController enemyMovementController, MainBase mainBase, MeleeEnemy meleeEnemy, TextMeshPro aıText)
        : base(animationController, interval, combatManager, model)
    {
        _combatManager = combatManager;
        _eventBus = eventBus;
        _damage = damage;
        _enemyMovementController = enemyMovementController;
        _mainBase = mainBase;
        _meleeEnemy = meleeEnemy;
        _aıText = aıText;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _eventBus.Subscribe<OnCharacterAttacked>(OnEnemyAttacked);
        _enemyMovementController.StopCharacter(false);
        _aıText.text = "Melee Attacking";
        // _meleeEnemy'i _mainBase'e döndür

    }

    public override void OnExit()
    {
        base.OnExit();
        _eventBus.Unsubscribe<OnCharacterAttacked>(OnEnemyAttacked);
    }
    
    public override void Tick()
    {
        base.Tick();
        // Vector3 direction = (_mainBase.transform.position - _meleeEnemy.transform.position).normalized;
        // if (direction != Vector3.zero)
        //     _targetRotation = Quaternion.LookRotation(direction);
        // _meleeEnemy.transform.rotation = Quaternion.Lerp(
        //     _meleeEnemy.transform.rotation,
        //     _targetRotation,
        //     Time.deltaTime * _rotationSpeed
        // );
    }

    private void OnEnemyAttacked(OnCharacterAttacked attackedCharacter)
    {
        if(attackedCharacter.AttackedCharacter != _combatManager.Character) return;
        if(_mainBase.IsCharacterDead) return;
        _mainBase.CharacterCombatManager.GetDamage(_damage);
    }
}