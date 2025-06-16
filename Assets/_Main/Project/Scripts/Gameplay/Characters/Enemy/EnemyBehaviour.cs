using System;
using System.Collections.Generic;
using AI.Base;
using AI.EnemyStates;
using CommonComponents;
using Pathfinding;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters.Enemy
{
    public class EnemyBehaviour : Character
    {
        private StateMachine _stateMachine;
        private AIDestinationSetter _aiDestinationSetter;
        private AIPath _aiPath;
        private Transform _playerTransform;
        private Collider2D _collider;
        private CamerasManager _camerasManager;
        [SerializeField] private List<GameObject> parts;


        protected override void GetComponents()
        {
            base.GetComponents();
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _aiPath = GetComponent<AIPath>();
            _collider = GetComponent<Collider2D>();
        }

        [Inject]
        private void Inject(CamerasManager camerasManager)
        {
            _camerasManager = camerasManager;
        }
        
        protected override void Start()
        {
            base.Start();
            SetStates();
        }

        private void SetStates()
        {
            _stateMachine = new StateMachine();

            #region States

            var waitingEnemyToArrive = new Waiting();
            var searchingForEnemy = new SearchingForEnemy();
            var walkingTowardsPlayer = new WalkingTowardsEnemy(AnimationController, _aiPath, model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed), CharacterCombatManager, CharacterDataHolder);
            var attacking = new Attacking(AnimationController, CharacterDataHolder.AttackingInterval);
            var fleeing = new Fleeing(AnimationController, _aiPath, CharacterSpeedController, CharacterCombatManager, _aiDestinationSetter, transform);
            var dead = new Dead(AnimationController, _collider, _aiPath, _camerasManager, parts, _playerTransform);
            #endregion

            #region State Changing Conditions

            Func<bool> ReachedEnemy() => () => _aiPath.remainingDistance < 1f && !IsCharacterDead;
            Func<bool> EnemyMovedFurther() => () => _aiPath.remainingDistance > 1f && !IsCharacterDead;
            Func<bool> IsFleeingEnabled() => () => CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            Func<bool> FleeingEnded() => () => !CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            Func<bool> CharacterIsDead() => () => IsCharacterDead;
            Func<bool> FoundEnemyNearby() => () => CharacterCombatManager.FindNearestEnemy() != null && !IsCharacterDead;

            #endregion

            #region Transitions

            _stateMachine.AddTransition(searchingForEnemy, walkingTowardsPlayer, FoundEnemyNearby());
            _stateMachine.AddTransition(walkingTowardsPlayer, attacking, ReachedEnemy());
            _stateMachine.AddTransition(attacking, walkingTowardsPlayer, EnemyMovedFurther());
            _stateMachine.AddTransition(fleeing, walkingTowardsPlayer, FleeingEnded());
            _stateMachine.AddAnyTransition(fleeing, IsFleeingEnabled());
            _stateMachine.AddAnyTransition(dead, CharacterIsDead());
            
            
            #endregion
            
            _stateMachine.SetState(searchingForEnemy);
        }
        
        private void Update()
        {
            _stateMachine.Tick();
        }
    }
}