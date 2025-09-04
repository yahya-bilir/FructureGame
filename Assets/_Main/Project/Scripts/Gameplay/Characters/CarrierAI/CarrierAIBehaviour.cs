using System;
using AI.Base;
using AI.CarrierAI;
using BasicStackSystem;
using CollectionSystem;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using WeaponSystem.RangedWeapons;

namespace Characters.CarrierAI
{
    public class CarrierAIBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform carryingPosition;
        [SerializeField] private int indexOfConnectedLoadingPoint;

        private CarryingController _carryingController;
        private StateMachine _stateMachine;
        private NavMeshAgent _navmeshAgent;
        private Animator _animator;
        private PhysicsStack _stack;
        private RangedWeaponWithExternalAmmo _weapon;
        private AmmoCreator _ammoCreator;
        public Vector3 ClosestPosition { get; set; }

        [Inject]
        private void Inject(AmmoCreator ammoCreator, PhysicsStack stack)
        {
            _ammoCreator = ammoCreator;
            _stack = stack;
        }
        private void Awake()
        {
            GetComponents();
        }

        protected void Start()
        {
            SetupStates();
        }

        public void Initialize(RangedWeaponWithExternalAmmo weapon)
        {
            _weapon = weapon;
            _carryingController = new CarryingController(carryingPosition, _stack, _animator, _weapon, _ammoCreator, indexOfConnectedLoadingPoint);
        }

        private void GetComponents()
        {
            _navmeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void SetupStates()
        {
            _stateMachine = new StateMachine();

            var waitingForStack = new WaitingForStack(_navmeshAgent, _animator);
            var walkingTowardsCarryingPosition = new WalkingTowardsCarryingPosition(_navmeshAgent, _animator, _carryingController, this);
            var collectingAmmo = new CollectingAmmo(_carryingController, _animator, _navmeshAgent);
            var carryingTowardsWeapon = new CarryingTowardsWeapon(_navmeshAgent, _animator, _weapon, _carryingController);
            var droppingAmmo = new DroppingAmmo(_carryingController, _animator, _navmeshAgent);
            var waitingWeaponToShoot = new WaitingWeaponToShoot(_navmeshAgent, _animator);

            Func<bool> ReachedStackAndThereIsAmmo() => () => Vector3.Distance(transform.position, ClosestPosition) <= 2f && !_carryingController.IsCarrying && _stack.IsThereAnyObject;
            Func<bool> ReachedStackButThereIsNoAmmo() => () => Vector3.Distance(transform.position, ClosestPosition) <= 2f && !_carryingController.IsCarrying && !_stack.IsThereAnyObject;
            Func<bool> IsThereAnyStackObject() => () => _stack.IsThereAnyObject && !_carryingController.IsCarrying;
            Func<bool> IsReachedWeapon() => () => Vector3.Distance(transform.position, _weapon.CarrierDropPoint.position) <= 0.75f && _carryingController.IsCarrying && _weapon.IsLoaded;
            Func<bool> IsReachedWeaponButWeaponIsLoaded() => () => Vector3.Distance(transform.position, _weapon.CarrierDropPoint.position) <= 0.75f && _carryingController.IsCarrying && _weapon.IsLoaded;
            Func<bool> IsWeaponEmpty() => () => !_weapon.IsLoaded;
            Func<bool> IsCarrying() => () => _carryingController.IsCarrying;
            Func<bool> IsDropped() => () => !_carryingController.IsCarrying;

            _stateMachine.AddTransition(waitingForStack, walkingTowardsCarryingPosition, IsThereAnyStackObject());            
            _stateMachine.AddTransition(walkingTowardsCarryingPosition, waitingForStack, ReachedStackButThereIsNoAmmo());            
            _stateMachine.AddTransition(walkingTowardsCarryingPosition, collectingAmmo, ReachedStackAndThereIsAmmo());            
            _stateMachine.AddTransition(collectingAmmo, carryingTowardsWeapon, IsCarrying());
            _stateMachine.AddTransition(carryingTowardsWeapon, droppingAmmo, IsReachedWeapon());
            _stateMachine.AddTransition(carryingTowardsWeapon, waitingWeaponToShoot, IsReachedWeaponButWeaponIsLoaded());
            _stateMachine.AddTransition(droppingAmmo, waitingForStack, IsDropped());
            _stateMachine.AddTransition(waitingWeaponToShoot, droppingAmmo, IsWeaponEmpty());
            
            _stateMachine.SetState(waitingForStack);
        }
        
        private void Update()
        {
            _stateMachine.Tick();
        }

    }
}