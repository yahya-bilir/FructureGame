using Characters;
using EventBusses;
using Events;
using UnityEngine;
using WeaponSystem.Managers;

public class RangedAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;
    private readonly IEventBus _eventBus;
    private readonly CharacterWeaponManager _characterWeaponManager;

    public RangedAttacking(CharacterAnimationController animationController, float interval,
        CharacterCombatManager combatManager, IEventBus eventBus, CharacterWeaponManager characterWeaponManager,
        GameObject model)
        : base(animationController, interval, combatManager, model)
    {
        _combatManager = combatManager;
        _eventBus = eventBus;
        _characterWeaponManager = characterWeaponManager;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        _eventBus.Subscribe<OnCharacterAttacked>(OnEnemyAttacked);
    }

    private void OnEnemyAttacked(OnCharacterAttacked eventData)
    {
        if(eventData.AttackedCharacter != _combatManager.Character) return;
        var lastFoundEnemy = _combatManager.LastFoundEnemy;
        if(lastFoundEnemy == null && lastFoundEnemy.IsCharacterDead) return;
        var item = _characterWeaponManager.SpawnedWeapon as RangedWeapon;
        item.Shoot(_combatManager.LastFoundEnemy);
    }

    public override void OnExit()
    {
        base.OnExit();
        _eventBus.Unsubscribe<OnCharacterAttacked>(OnEnemyAttacked);
    }
}