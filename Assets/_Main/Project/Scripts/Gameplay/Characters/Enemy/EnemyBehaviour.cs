using System;
using AI.Base;
using AI.EnemyStates;
using Pathfinding;
using PropertySystem;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyBehaviour : Character
    {
        private StateMachine _stateMachine;
        private AIDestinationSetter _aiDestinationSetter;
        private AIPath _aiPath;
        private EnemyAnimationController _animationController;
        private Transform _playerTransform;

        private bool IsCharacterDead =>
            CharacterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue <= 0;
        protected override void Awake()
        {
            base.Awake();
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _aiPath = GetComponent<AIPath>();
            _animationController = new EnemyAnimationController(animator);
            CharacterCombatManager = new CharacterCombatManager(this, CharacterPropertyManager, CharacterDataHolder);
        }

        private void Start()
        {
            SetStates();
        }

        public void InitializeOnSpawn(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _aiDestinationSetter.target = _playerTransform;
            
        }
        
        private void SetStates()
        {
            _stateMachine = new StateMachine();

            #region States

            var walkingTowardsPlayer = new WalkingTowardsPlayer(_animationController, _playerTransform, _aiPath, model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed));
            var attacking = new Attacking(_animationController);
            var dead = new Dead(_animationController);
            #endregion

            #region State Changing Conditions

            Func<bool> ReachedPlayer() => () => _aiPath.remainingDistance < 0.5f;
            Func<bool> PlayerMovedFurther() => () => _aiPath.remainingDistance > 1f;
            Func<bool> CharacterIsDead() => () => IsCharacterDead;
            #endregion

            #region Transitions

            _stateMachine.AddTransition(walkingTowardsPlayer, attacking, ReachedPlayer());
            _stateMachine.AddTransition(attacking, walkingTowardsPlayer, PlayerMovedFurther());
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