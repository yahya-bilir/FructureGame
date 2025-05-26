using System;
using AI.Base;
using AI.EnemyStates;
using Characters.Player;
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


        protected override void GetComponents()
        {
            base.GetComponents();
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _aiPath = GetComponent<AIPath>();
            _collider = GetComponent<Collider2D>();
        }

        [Inject]
        private void Inject(PlayerController playerController)
        {
            _playerTransform = playerController.transform;
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

            var walkingTowardsPlayer = new WalkingTowardsPlayer(AnimationController, _playerTransform, _aiPath, model.transform, CharacterPropertyManager.GetProperty(PropertyQuery.Speed));
            var attacking = new Attacking(AnimationController, CharacterDataHolder.AttackingInterval, _playerCombatManager, CharacterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
            var dead = new Dead(AnimationController, _collider, _aiPath, _aiDestinationSetter, GetComponent<RVOController>());
            #endregion

            #region State Changing Conditions

            Func<bool> ReachedPlayer() => () => _aiPath.remainingDistance < 0.75f;
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