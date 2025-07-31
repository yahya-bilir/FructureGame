using System;
using System.Collections.Generic;
using System.Linq;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using Characters.BaseSystem;
using EventBusses;
using PropertySystem;
using TMPro;
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
        protected MainBase MainBase;

        private EnemyRigidbodyEffectsController _rigidbodyEffectsController;
        private AttackAnimationCaller _attackAnimationCaller;

        private bool _isCrushed;
        private bool _isKnockbacked;
        private List<Renderer> _renderers;

        [Inject]
        private void Inject(IEventBus eventBus, MainBase mainBase)
        {
            EventBus = eventBus;
            MainBase = mainBase;
        }

        protected override void Awake()
        {
            base.Awake();
            CharacterVisualEffects = new EnemyVisualEffects(healthBar, onDeathVfx, this, AnimationController, hitVfx, Feedback, _renderers);
        }

        protected override void Start()
        {
            base.Start();

            Collider.enabled = true;

            EnemyMovementController = new EnemyMovementController(
                Collider,
                _rigidbody,
                AnimationController,
                this,
                model,
                CharacterPropertyManager.GetProperty(PropertyQuery.Speed),
                _navmeshAgent
            );

            _rigidbodyEffectsController = new EnemyRigidbodyEffectsController(model, _rigidbody, this);
            _rigidbodyEffectsController.Initialize();

            SetupStates();

            Resolver.Inject(_attackAnimationCaller);
            Resolver.Inject(_rigidbodyEffectsController);

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
            _renderers = model.GetComponentsInChildren<Renderer>().ToList();
        }

        private void SetupStates()
        {
            StateMachine = new StateMachine();

            WalkingToEnemy = new WalkingTowardsEnemy(MainBase, CharacterDataHolder, EnemyMovementController, model.transform, AIText);
            AttackingState = CreateAttackingState();

            var dead = new Dead(AnimationController, Collider, AIText, EnemyMovementController);
            var crushed = new Crushed(Collider, AIText, EnemyMovementController, CharacterCombatManager, AnimationController);
            var knockbacked = new Knockbacked(AIText, EnemyMovementController, _rigidbody, this, AnimationController);

            Func<bool> ReachedEnemy() => () =>
                Vector3.Distance(transform.position, MainBase.transform.position) <= 1f && !IsCharacterDead;

            Func<bool> IsDead() => () => IsCharacterDead && !_isCrushed;

            Func<bool> IsCrushed() => () => _isCrushed && !_isKnockbacked && !IsCharacterDead;

            Func<bool> ShouldKnockback() => () => _isKnockbacked && !_isCrushed && !IsCharacterDead;

            Func<bool> KnockbackComplete() => () => knockbacked.KnockbackTimer >= 0.85f && !_isCrushed && !IsCharacterDead;

            StateMachine.AddTransition(WalkingToEnemy, AttackingState, ReachedEnemy());
            StateMachine.AddTransition(WalkingToEnemy, knockbacked, ShouldKnockback());
            StateMachine.AddTransition(knockbacked, WalkingToEnemy, KnockbackComplete());
            StateMachine.AddAnyTransition(dead, IsDead());
            StateMachine.AddAnyTransition(crushed, IsCrushed());

            AddCustomStatesAndTransitions(StateMachine);

            StateMachine.SetState(WalkingToEnemy);
        }

        public void SetCrushed() => _isCrushed = true;

        public void SetKnockbacked(bool isKnockbacked)
        {
            _isKnockbacked = isKnockbacked;
        }

        protected abstract BaseAttacking CreateAttackingState();

        protected virtual void AddCustomStatesAndTransitions(StateMachine sm) { }
    }
}
