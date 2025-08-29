using System;
using System.Collections.Generic;
using System.Linq;
using AI.Base;
using AI.Base.Interfaces;
using AI.EnemyStates;
using Characters.BaseSystem;
using EventBusses;
using PropertySystem;
using RayFire;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Characters.Enemy
{
    public abstract class EnemyBehaviour : Character
    {
        [SerializeField] private GameObject fireVfxObj;
        [SerializeField] private GameObject electricVfxObj;
        [SerializeField] private List<MeshColliderAndSkinnedMeshData> meshColliderAndSkinnedMeshDatas;
        
        protected Collider Collider;
        protected CharacterMovementController CharacterMovementController;
        protected IEventBus EventBus;
        private Rigidbody _rigidbody;
        protected StateMachine StateMachine;
        protected IState AttackingState;
        private NavMeshAgent _navmeshAgent;
        protected IState WalkingToEnemy;
        protected MainBase MainBase;

        private AttackAnimationCaller _attackAnimationCaller;
        private Dictionary<DamageTypes, GameObject> _damageAndGameObjects = new Dictionary<DamageTypes, GameObject>();
        public EnemyDestructionManager EnemyDestructionManager { get; private set; }
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
            _damageAndGameObjects.Add(DamageTypes.Fire, fireVfxObj);
            _damageAndGameObjects.Add(DamageTypes.Electric, electricVfxObj);
            EnemyDestructionManager = new EnemyDestructionManager(meshColliderAndSkinnedMeshDatas, AnimationController);
            CharacterVisualEffects = new EnemyVisualEffects(healthBar, onDeathVfx, this, AnimationController, 
                hitVfx, Feedback, _renderers, spawnVfx, _damageAndGameObjects, EnemyDestructionManager);
        }

        protected override void Start()
        {
            base.Start();
            CharacterCombatManager = new EnemyCombatManager(CharacterPropertyManager, CharacterVisualEffects, this,
                EnemyDestructionManager);
            Collider.enabled = true;

            CharacterMovementController = new CharacterMovementController(
                Collider,
                _rigidbody,
                AnimationController,
                this,
                model,
                CharacterPropertyManager.GetProperty(PropertyQuery.Speed),
                _navmeshAgent
            );
            

            SetupStates();

            Resolver.Inject(_attackAnimationCaller);
            Resolver.Inject(CharacterCombatManager);
            Resolver.Inject(EnemyDestructionManager);

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

            WalkingToEnemy = new WalkingTowardsEnemy(MainBase, CharacterDataHolder, CharacterMovementController, model.transform, AIText);
            AttackingState = CreateAttackingState();
            var dead = new Dead(AnimationController, Collider, AIText, CharacterMovementController);

            Func<bool> ReachedEnemy() => () =>
                Vector3.Distance(transform.position, MainBase.Collider.ClosestPoint(transform.position)) <= 0.2f && !IsCharacterDead;

            Func<bool> IsDead() => () => IsCharacterDead;


            StateMachine.AddTransition(WalkingToEnemy, AttackingState, ReachedEnemy());
            StateMachine.AddAnyTransition(dead, IsDead());
            AddCustomStatesAndTransitions(StateMachine);

            StateMachine.SetState(WalkingToEnemy);
        }

        protected abstract BaseAttacking CreateAttackingState();

        protected virtual void AddCustomStatesAndTransitions(StateMachine sm) { }
    }
}
