using System;
using System.Collections.Generic;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using CommonComponents;
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

    [SerializeField] private List<GameObject> parts;

    protected IState walkingState;
    protected IState attackingState;

    [Inject]
    private void Inject(CamerasManager camerasManager)
    {
        _camerasManager = camerasManager;
    }

    protected override void GetComponents()
    {
        base.GetComponents();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiPath = GetComponent<AIPath>();
        _collider = GetComponent<Collider2D>();
    }

    protected override void Start()
    {
        base.Start();
        SetupStates();
    }

    private void SetupStates()
    {
        _stateMachine = new StateMachine();

        var waiting = new Waiting();
        var searching = new SearchingForEnemy();
        walkingState = CreateWalkingState();
        attackingState = CreateAttackingState(); 

        var fleeing = new Fleeing(AnimationController, _aiPath, CharacterSpeedController, CharacterCombatManager, _aiDestinationSetter, transform);
        var dead = new Dead(AnimationController, _collider, _aiPath, _camerasManager, parts, transform);

        // --- Geçiş Koşulları ---
        Func<bool> FoundEnemyNearby() => () => CharacterCombatManager.FindNearestEnemy() != null && !IsCharacterDead;
        Func<bool> ReachedEnemy() => () => _aiPath.remainingDistance < 1f && !IsCharacterDead;
        Func<bool> EnemyMovedFurther() => () => _aiPath.remainingDistance > 1f && !IsCharacterDead;
        Func<bool> IsFleeingEnabled() => () => CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
        Func<bool> FleeingEnded() => () => !CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
        Func<bool> CharacterIsDead() => () => IsCharacterDead;

        _stateMachine.AddTransition(searching, walkingState, FoundEnemyNearby());
        _stateMachine.AddTransition(walkingState, attackingState, ReachedEnemy());
        _stateMachine.AddTransition(attackingState, walkingState, EnemyMovedFurther());
        _stateMachine.AddTransition(fleeing, walkingState, FleeingEnded());
        _stateMachine.AddAnyTransition(fleeing, IsFleeingEnabled());
        _stateMachine.AddAnyTransition(dead, CharacterIsDead());

        AddCustomStatesAndTransitions(_stateMachine);

        _stateMachine.SetState(searching);
    }

    protected virtual IState CreateWalkingState()
    {
        return new WalkingTowardsEnemy(AnimationController, _aiPath, model.transform,
            CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterCombatManager, CharacterDataHolder);
    }

    protected abstract IState CreateAttackingState();

    // 🔁 Ek state geçişleri için override edilebilir
    protected virtual void AddCustomStatesAndTransitions(StateMachine sm) { }

    private void Update()
    {
        _stateMachine.Tick();
    }
}



}