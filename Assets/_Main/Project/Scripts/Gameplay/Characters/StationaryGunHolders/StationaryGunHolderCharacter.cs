using AI.Base;
using BasicStackSystem;
using Characters;
using CollectionSystem;
using EventBusses;
using PropertySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using WeaponSystem.RangedWeapons;

public class StationaryGunHolderCharacter : Character
{
    protected StateMachine _stateMachine;
    private SearchingForEnemy _searchingState;
    private Attacking _attackingState;

    protected RangedWeapon _rangedWeapon;
    protected Transform _weaponTransform;
    protected IEventBus _eventBus;
    [SerializeField] private BasicStack connectedStack;
    private AmmoCreator _ammoCreator;

    [Inject]
    private void Inject(IEventBus eventBus, AmmoCreator ammoCreator)
    {
        _eventBus = eventBus;
        _ammoCreator = ammoCreator;
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

        if (_rangedWeapon is RangedWeaponWithExternalAmmo externalAmmoWeapon)
        {
            externalAmmoWeapon.SetStack(connectedStack);
            _ammoCreator.OnRangedWeaponCreated(externalAmmoWeapon);
        }
        
        SetStates();
    }

    protected virtual void SetStates()
    {
        _searchingState = new SearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, this.name);
        _attackingState = new Attacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus, connectedStack);

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

    [Button]
    public void DebugUpgradeAttackSpeed()
    {
        var speedValue = CharacterPropertyManager.GetProperty(PropertyQuery.AttackSpeed).TemporaryValue;
        CharacterPropertyManager.SetPropertyTemporarily(PropertyQuery.AttackSpeed, speedValue + 0.1f);
        Debug.Log(CharacterPropertyManager.GetProperty(PropertyQuery.AttackSpeed).TemporaryValue);
    }
}