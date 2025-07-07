using System;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using CommonComponents;
using EventBusses;
using IslandSystem;
using Pathfinding;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters.Enemy
{
public abstract class EnemyBehaviour : Character
{
    protected StateMachine _stateMachine;
    protected AIPath _aiPath;
    protected AIDestinationSetter _aiDestinationSetter;
    protected Collider2D _collider;
    protected CamerasManager _camerasManager;
    
    protected IState walkingToEnemy;
    protected IState attackingState;
    protected IEventBus _eventBus;
    private IslandManager _islandManager;
    private Rigidbody2D _rigidbody2D;

    [Inject]
    private void Inject(CamerasManager camerasManager, IEventBus eventBus, IslandManager islandManager)
    {
        _camerasManager = camerasManager;
        _eventBus = eventBus;
        _islandManager = islandManager;
    }

    protected override void GetComponents()
    {
        base.GetComponents();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiPath = GetComponent<AIPath>();
        _collider = GetComponent<Collider2D>();
        _rigidbody2D =  GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        SetupStates();
    }

    private void SetupStates()
    {
        _stateMachine = new StateMachine();
        
        var waiting = new Waiting(_collider, _eventBus, _aiPath, AnimationController);
        var walkingTowardsJumpingPosition = new WalkingTowardsJumpingPosition(AnimationController, _aiPath, model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterIslandController);
        var jumpingToPosition = new JumpingToPosition(CharacterIslandController, _aiPath, model.transform, this, AnimationController);
        var searching = new SearchingForEnemy(_collider, _aiPath, _rigidbody2D, AnimationController);
        walkingToEnemy = CreateWalkingState();
        attackingState = CreateAttackingState(); 

        var fleeing = new Fleeing(AnimationController, _aiPath, CharacterSpeedController, CharacterCombatManager, _aiDestinationSetter, transform);
        var dead = new Dead(AnimationController, _collider, _aiPath, _camerasManager, CharacterDataHolder.OnDeathParts, transform);

        Func<bool> FoundEnemyNearby() => () => CharacterCombatManager.FindNearestEnemy() != null && !IsCharacterDead;
        Func<bool> ReachedEnemy() => () => _aiPath.remainingDistance < 0.25f && !IsCharacterDead;
        Func<bool> ReachedJumpingPosition() => () => _aiPath.remainingDistance <= 1f && !IsCharacterDead && _aiPath.canMove;
        Func<bool> CanJump() => () => !IsCharacterDead && CharacterIslandController.CanJump;
        Func<bool> EnemyMovedFurther() => () => _aiPath.remainingDistance > 0.25f && !IsCharacterDead;
        Func<bool> IsFleeingEnabled() => () => CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
        Func<bool> FleeingEnded() => () => !CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
        Func<bool> CharacterIsDead() => () => IsCharacterDead;
        Func<bool> OnFightStarted() => () => _islandManager.FightCanStart && !IsCharacterDead && !CharacterIslandController.IsJumping;
        Func<bool> EnemyDied() => () =>
            CharacterCombatManager.LastFoundEnemy != null && CharacterCombatManager.LastFoundEnemy.IsCharacterDead && 
            !IsCharacterDead;
        Func<bool> IslandChanged() => () => CharacterIslandController.PreviousIsland != CharacterIslandController.NextIsland && !IsCharacterDead;

        _stateMachine.AddTransition(waiting, searching, OnFightStarted());
        _stateMachine.AddTransition(searching, walkingToEnemy, FoundEnemyNearby());
        _stateMachine.AddTransition(walkingToEnemy, attackingState, ReachedEnemy());
        _stateMachine.AddTransition(attackingState, walkingToEnemy, EnemyMovedFurther());
        _stateMachine.AddTransition(walkingToEnemy, searching, EnemyDied());
        _stateMachine.AddTransition(attackingState, searching, EnemyDied());
        _stateMachine.AddTransition(fleeing, walkingToEnemy, FleeingEnded());
        _stateMachine.AddTransition(searching, walkingTowardsJumpingPosition, IslandChanged());
        //_stateMachine.AddTransition(walkingTowardsJumpingPosition, jumpingToPosition, ReachedJumpingPosition());
        _stateMachine.AddTransition(walkingTowardsJumpingPosition, waiting, ReachedJumpingPosition());
        _stateMachine.AddTransition(waiting, jumpingToPosition, CanJump());
        _stateMachine.AddTransition(jumpingToPosition, searching, OnFightStarted());
        
        _stateMachine.AddAnyTransition(fleeing, IsFleeingEnabled());
        _stateMachine.AddAnyTransition(dead, CharacterIsDead());

        AddCustomStatesAndTransitions(_stateMachine);


        _stateMachine.SetState(waiting);
        
    }

    protected virtual IState CreateWalkingState()
    {
        return new WalkingTowardsEnemy(AnimationController, _aiPath, model.transform,
            CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterCombatManager, CharacterDataHolder, _collider);
    }

    protected abstract BaseAttacking CreateAttackingState();

    protected virtual void AddCustomStatesAndTransitions(StateMachine sm) { }

    private void Update()
    {
        _stateMachine.Tick();
    }
}



}