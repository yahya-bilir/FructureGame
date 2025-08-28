using AI.Base;
using AI.StationaryGunHolderStates;
using BasicStackSystem;
using Characters;
using Characters.StationaryGunHolders;
using CollectionSystem;
using EventBusses;
using UnityEngine;
using VContainer;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

public class StationaryGunHolderCharacter : Character
{
    protected StateMachine _stateMachine;
    private SearchingForEnemy _searchingState;
    private Attacking _attackingState;

    protected RangedWeaponWithExternalAmmo _rangedWeapon;
    protected Transform _weaponTransform;
    protected IEventBus _eventBus;
    private AmmoCreator _ammoCreator;
    private GunHolderEventHandler _gunHolderEventHandler;
    
    [SerializeField] private BasicStack connectedStack;
    [SerializeField] private AmmoBase scriptToAddWhenCollected;
    
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

        _rangedWeapon = CharacterWeaponManager.SpawnedWeapon as RangedWeaponWithExternalAmmo;
        _weaponTransform = CharacterWeaponManager.SpawnedWeapon?.transform;

        if (_rangedWeapon == null || _weaponTransform == null)
        {
            Debug.LogError("Weapon not properly initialized.");
            return;
        }

        _rangedWeapon.SetStack(connectedStack);
        _ammoCreator.OnRangedWeaponCreated(connectedStack, _rangedWeapon.RangedWeaponSo.ProjectilePrefab);

        _gunHolderEventHandler = new GunHolderEventHandler(this, CharacterPropertyManager);
        Resolver.Inject(_gunHolderEventHandler);
        SetStates();
    }

    protected virtual void SetStates()
    {
        _searchingState = new SearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, this.name);
        _attackingState = new Attacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus, connectedStack);
        var waitingForWeaponToBeLoaded = new WaitingForWeaponToBeLoaded(_rangedWeapon, AIText);
        _stateMachine.AddTransition(
            _searchingState,
            waitingForWeaponToBeLoaded,
            () => LastEnemyIsValid()
        );        
        
        _stateMachine.AddTransition(
            waitingForWeaponToBeLoaded,
            _attackingState,
            () => _rangedWeapon.IsLoaded && LastEnemyIsValid()
        );
        
        _stateMachine.AddTransition(
            _attackingState,
            waitingForWeaponToBeLoaded,
            () => !_rangedWeapon.IsLoaded && LastEnemyIsValid()
        );
        
        _stateMachine.AddAnyTransition(_searchingState, () => !LastEnemyIsValid());
        
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

    // [Button]
    // public void DebugUpgradeAttackSpeed()
    // {
    //     var speedValue = CharacterPropertyManager.GetProperty(PropertyQuery.AttackSpeed).TemporaryValue;
    //     CharacterPropertyManager.SetPropertyTemporarily(PropertyQuery.AttackSpeed, speedValue + 0.1f);
    //     Debug.Log(CharacterPropertyManager.GetProperty(PropertyQuery.AttackSpeed).TemporaryValue);
    // }
}