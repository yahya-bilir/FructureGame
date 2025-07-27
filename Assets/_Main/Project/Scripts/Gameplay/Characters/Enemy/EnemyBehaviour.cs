using System;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using Characters.BaseSystem;
using EventBusses;
using PropertySystem;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Characters.Enemy
{
    public abstract class EnemyBehaviour : Character
    {
        protected Collider Collider;
        protected EnemyMovementController EnemyMovementController;
        protected IEventBus EventBus;
        private Rigidbody _rigidbody;
        protected StateMachine StateMachine;
        protected IState AttackingState;
        private NavMeshAgent _navmeshAgent;
        protected IState WalkingToEnemy;
        private MainBase _mainBase;
        private EnemyRagdollManager _ragdollManager;
        private AttackAnimationCaller _attackAnimationCaller;

        [Inject]
        private void Inject(IEventBus eventBus, MainBase mainBase)
        {
            EventBus = eventBus;
            _mainBase = mainBase;
        }

        protected override void Start()
        {
            base.Start();
            EnemyMovementController = new EnemyMovementController(Collider, 
                _rigidbody, AnimationController, this,
                model, 
                CharacterPropertyManager.GetProperty(PropertyQuery.Speed), _navmeshAgent);
            
            _ragdollManager = new EnemyRagdollManager(model, AnimationController);
            _ragdollManager.Initialize();
            SetupStates();
            
            Resolver.Inject(_attackAnimationCaller);
            _navmeshAgent.SetDestination(Vector3.zero);
        }

        private void Update()
        {
            StateMachine.Tick();
        }

        protected override void GetComponents()
        {
            base.GetComponents();
            Collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _navmeshAgent = GetComponent<NavMeshAgent>();
            _attackAnimationCaller = GetComponentInChildren<AttackAnimationCaller>();
            
        }

        private void SetupStates()
        {
            StateMachine = new StateMachine();
            
            WalkingToEnemy = new WalkingTowardsEnemy(_mainBase, CharacterDataHolder, EnemyMovementController, model.transform);
            AttackingState = CreateAttackingState();
            var dead = new Dead(AnimationController, Collider);
            
            Func<bool> ReachedEnemy()
            {
                return () =>
                    Vector3.Distance(transform.position, _mainBase.transform.position) <= 0.5f && !IsCharacterDead;
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

        protected abstract BaseAttacking CreateAttackingState();

        protected virtual void AddCustomStatesAndTransitions(StateMachine sm)
        {
        }
    }
}