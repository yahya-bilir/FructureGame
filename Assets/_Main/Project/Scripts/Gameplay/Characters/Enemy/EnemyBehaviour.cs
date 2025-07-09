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
        protected AIDestinationSetter _aiDestinationSetter;
        protected AIPath _aiPath;
        protected CamerasManager _camerasManager;
        protected Collider2D _collider;
        protected IEventBus _eventBus;
        private IslandManager _islandManager;
        private Rigidbody2D _rigidbody2D;
        protected StateMachine _stateMachine;
        protected IState attackingState;

        protected IState walkingToEnemy;

        protected override void Start()
        {
            base.Start();
            SetupStates();
        }

        private void Update()
        {
            _stateMachine.Tick();
        }

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
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void SetupStates()
        {
            _stateMachine = new StateMachine();

            var waiting = new Waiting(_collider, _eventBus, _aiPath, AnimationController);
            var walkingTowardsJumpingPosition = new WalkingTowardsJumpingPosition(AnimationController, _aiPath,
                model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterIslandController,
                _collider);
            var jumpingToPosition = new JumpingToPosition(CharacterIslandController, _aiPath, model.transform, this,
                AnimationController, _collider);
            var searching = new SearchingForEnemy(_collider, _aiPath, _rigidbody2D, AnimationController);
            walkingToEnemy = CreateWalkingState();
            attackingState = CreateAttackingState();

            var fleeing = new Fleeing(AnimationController, _aiPath, CharacterSpeedController, CharacterCombatManager,
                _aiDestinationSetter, transform);
            var dead = new Dead(AnimationController, _collider, _aiPath, _camerasManager,
                CharacterDataHolder.OnDeathParts, transform);

            Func<bool> FoundEnemyNearby()
            {
                return () => CharacterCombatManager.FindNearestEnemy() != null && !IsCharacterDead;
            }

            Func<bool> ReachedEnemy()
            {
                return () => _aiPath.remainingDistance < _aiPath.endReachedDistance && !IsCharacterDead;
            }

            Func<bool> ReachedJumpingPosition()
            {
                return () => _aiPath.remainingDistance <= 0.1f && !IsCharacterDead && _aiPath.canMove;
            }

            Func<bool> CanJump()
            {
                return () => !IsCharacterDead && CharacterIslandController.CanJump;
            }

            Func<bool> EnemyMovedFurther()
            {
                return () => _aiPath.remainingDistance > _aiPath.endReachedDistance + 0.1f && !IsCharacterDead;
            }

            Func<bool> IsFleeingEnabled()
            {
                return () => CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            }

            Func<bool> FleeingEnded()
            {
                return () => !CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            }

            Func<bool> CharacterIsDead()
            {
                return () => IsCharacterDead;
            }

            Func<bool> OnFightStarted()
            {
                return () => _islandManager.FightCanStart && !IsCharacterDead;
                /*&& !CharacterIslandController.IsJumping*/
            }

            Func<bool> EnemyDied()
            {
                return () =>
                    CharacterCombatManager.LastFoundEnemy != null &&
                    CharacterCombatManager.LastFoundEnemy.IsCharacterDead &&
                    !IsCharacterDead;
            }

            Func<bool> IslandChanged()
            {
                return () => CharacterIslandController.PreviousIsland != CharacterIslandController.NextIsland &&
                             !IsCharacterDead;
            }

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
                CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterCombatManager, CharacterDataHolder,
                _collider);
        }

        protected abstract BaseAttacking CreateAttackingState();

        protected virtual void AddCustomStatesAndTransitions(StateMachine sm)
        {
        }
    }
}