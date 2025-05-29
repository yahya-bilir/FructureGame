using System;
using System.Collections.Generic;
using AI.Base;
using AI.EnemyStates;
using Characters.Player;
using CommonComponents;
using EventBusses;
using Events;
using Pathfinding;
using Pathfinding.RVO;
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
        private CharacterCombatManager _playerCombatManager;
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
        private void Inject(PlayerController playerController, CamerasManager camerasManager)
        {
            _playerTransform = playerController.transform;
            _camerasManager = camerasManager;
        }
        
        protected override void Start()
        {
            base.Start();
            SetStates();
        }

        public void InitializeOnSpawn(CharacterCombatManager playerCombatManager)
        {
            _aiDestinationSetter.target = _playerTransform;
            _playerCombatManager = playerCombatManager;
        }
        
        private void SetStates()
        {
            _stateMachine = new StateMachine();

            #region States

            var walkingTowardsPlayer = new WalkingTowardsPlayer(AnimationController, _playerTransform, _aiPath, model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed), _aiDestinationSetter);
            var attacking = new Attacking(AnimationController, CharacterDataHolder.AttackingInterval, _playerCombatManager, CharacterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
            var fleeing = new Fleeing(AnimationController, _aiPath, CharacterSpeedController, CharacterCombatManager, _aiDestinationSetter, transform);
            var dead = new Dead(AnimationController, _collider, _aiPath, _camerasManager, parts, _playerTransform);
            #endregion

            #region State Changing Conditions

            Func<bool> ReachedPlayer() => () => _aiPath.remainingDistance < 0.75f && !IsCharacterDead;
            Func<bool> PlayerMovedFurther() => () => _aiPath.remainingDistance > 1f && !IsCharacterDead;
            Func<bool> IsFleeingEnabled() => () => CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            Func<bool> FleeingEnded() => () => !CharacterCombatManager.FleeingEnabled && !IsCharacterDead;
            Func<bool> CharacterIsDead() => () => IsCharacterDead;
            #endregion

            #region Transitions

            _stateMachine.AddTransition(walkingTowardsPlayer, attacking, ReachedPlayer());
            _stateMachine.AddTransition(attacking, walkingTowardsPlayer, PlayerMovedFurther());
            _stateMachine.AddTransition(fleeing, walkingTowardsPlayer, FleeingEnded());
            _stateMachine.AddAnyTransition(fleeing, IsFleeingEnabled());
            _stateMachine.AddAnyTransition(dead, CharacterIsDead());

            #endregion
            
            _stateMachine.SetState(walkingTowardsPlayer);
        }
        
        private void Update()
        {
            _stateMachine.Tick();
        }
        

    }
}