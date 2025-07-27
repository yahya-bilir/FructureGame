using System;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using CommonComponents;
using EventBusses;
using PropertySystem;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Characters.Enemy
{
    public abstract class EnemyBehaviour : Character
    {
        protected CamerasManager CamerasManager;
        protected Collider2D Collider;
        protected EnemyMovementController EnemyMovementController;
        protected IEventBus EventBus;
        private Rigidbody2D _rigidbody2D;
        protected StateMachine StateMachine;
        protected IState AttackingState;
        private NavMeshAgent _navmeshAgent;
        protected IState WalkingToEnemy;
        
        protected override void Start()
        {
            base.Start();
            EnemyMovementController = new EnemyMovementController(Collider, 
                _rigidbody2D, AnimationController, this,
                model, 
                CharacterPropertyManager.GetProperty(PropertyQuery.Speed), _navmeshAgent);
            SetupStates();
        }

        private void Update()
        {
            StateMachine.Tick();
        }

        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus)
        {
            CamerasManager = camerasManager;
            EventBus = eventBus;
        }

        protected override void GetComponents()
        {
            base.GetComponents();
            Collider = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void SetupStates()
        {
            StateMachine = new StateMachine();
            WalkingToEnemy = CreateWalkingState();
            AttackingState = CreateAttackingState();
            
            var dead = new Dead(AnimationController, Collider);

            Func<bool> ReachedEnemy()
            {
                return () =>
                    (_navmeshAgent.remainingDistance <= _navmeshAgent.stoppingDistance + 0.1f || _navmeshAgent.isStopped) && !IsCharacterDead;
            }
            
            Func<bool> CharacterIsDead()
            {
                return () => IsCharacterDead;
            }
            

            StateMachine.AddTransition(WalkingToEnemy, AttackingState, ReachedEnemy());
            StateMachine.AddAnyTransition(dead, CharacterIsDead());
            AddCustomStatesAndTransitions(StateMachine);
            StateMachine.SetState(WalkingToEnemy);
        }

        protected virtual IState CreateWalkingState()
        {
            return new WalkingTowardsEnemy(CharacterCombatManager, CharacterDataHolder, EnemyMovementController, model.transform);
        }

        protected abstract BaseAttacking CreateAttackingState();

        protected virtual void AddCustomStatesAndTransitions(StateMachine sm)
        {
        }
    }
}