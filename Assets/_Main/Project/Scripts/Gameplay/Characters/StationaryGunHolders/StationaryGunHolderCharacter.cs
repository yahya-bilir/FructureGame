using AI.Base;
using Characters;
using EventBusses;
using UnityEngine;
using VContainer;

public class StationaryGunHolderCharacter : Character
{
    private StateMachine _stateMachine;
    private SearchingForEnemy _searchingState;
    private Attacking _attackingState;

    private RangedWeapon _rangedWeapon;
    private Transform _weaponTransform;
    private IEventBus _eventBus;

    [Inject]
    private void Inject(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    protected override void Start()
    {
        base.Start();

        
        _stateMachine = new StateMachine();

        _rangedWeapon = CharacterWeaponManager.SpawnedWeapon as RangedWeapon;
        _weaponTransform = CharacterWeaponManager.SpawnedWeapon?.transform;

        if (_rangedWeapon == null || _weaponTransform == null)
        {
            Debug.LogError("Weapon not properly initialized.");
            return;
        }

        SetStates();
    }

    private void SetStates()
    {
        _searchingState = new SearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, this.name);
        _attackingState = new Attacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus);

        _stateMachine.AddTransition(
            _searchingState,
            _attackingState,
            () => LastEnemyIsValid()
        );

        _stateMachine.AddTransition(
            _attackingState,
            _searchingState,
            () => !LastEnemyIsValid()
        );

        _stateMachine.SetState(_searchingState);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    private bool LastEnemyIsValid()
    {
        var enemy = CharacterCombatManager.LastFoundEnemy;
        return enemy != null && !enemy.IsCharacterDead && !IsCharacterDead;
    }
}