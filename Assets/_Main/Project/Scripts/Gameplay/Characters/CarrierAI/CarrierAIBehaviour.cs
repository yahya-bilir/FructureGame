using System;
using AI.Base;
using AI.CarrierAI;
using BasicStackSystem;
using UnityEngine;
using UnityEngine.AI;
using WeaponSystem.RangedWeapons;

namespace Characters.CarrierAI
{
    public class CarrierAIBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform carryingPosition;
        
        private CarryingController _carryingController;
        private StateMachine _stateMachine;
        private NavMeshAgent _navmeshAgent;
        private Animator _animator;
        private BasicStack _stack;
        private RangedWeaponWithExternalAmmo _weapon;
        private void Awake()
        {
            GetComponents();
        }

        protected void Start()
        {
            SetupStates();
        }

        public void Initialize(BasicStack stack, RangedWeaponWithExternalAmmo weapon)
        {
            _weapon = weapon;
            _stack = stack;
            _carryingController = new CarryingController(carryingPosition, _stack, _animator, _weapon);
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
            var walkingTowardsCarryingPosition = new WalkingTowardsCarryingPosition(_navmeshAgent, _animator, _stack);
            var collectingAmmo = new CollectingAmmo(_carryingController, _animator, _navmeshAgent);
            var carryingTowardsWeapon = new CarryingTowardsWeapon(_navmeshAgent, _animator, _weapon);
            var droppingAmmo = new DroppingAmmo(_carryingController, _animator, _navmeshAgent);
            var waitingWeaponToShoot = new WaitingWeaponToShoot(_navmeshAgent, _animator);

            Func<bool> ReachedStack() => () => Vector3.Distance(transform.position, _stack.transform.position) <= 2f && !_carryingController.IsCarrying;
            Func<bool> IsThereAnyStackObject() => () => _stack.IsThereAnyObject && !_carryingController.IsCarrying;
            Func<bool> IsReachedWeapon() => () => Vector3.Distance(transform.position, _weapon.transform.position) <= 2f && _carryingController.IsCarrying;
            Func<bool> IsWeaponEmpty() => () => !_weapon.IsLoaded;
            Func<bool> IsCarrying() => () => _carryingController.IsCarrying;
            Func<bool> IsDropped() => () => !_carryingController.IsCarrying;

            _stateMachine.AddTransition(waitingForStack, walkingTowardsCarryingPosition, IsThereAnyStackObject());            
            _stateMachine.AddTransition(walkingTowardsCarryingPosition, collectingAmmo, ReachedStack());            
            _stateMachine.AddTransition(collectingAmmo, carryingTowardsWeapon, IsCarrying());
            _stateMachine.AddTransition(carryingTowardsWeapon, droppingAmmo, IsReachedWeapon());
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