using AI.Base.Interfaces;
using Characters.CarrierAI;
using UnityEngine;
using UnityEngine.AI;
using WeaponSystem.RangedWeapons;

namespace AI.CarrierAI
{
    public class CarryingTowardsWeapon : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private readonly NavMeshAgent _navmeshAgent;
        private readonly Animator _animator;
        private readonly RangedWeaponWithExternalAmmo _weapon;
        private readonly CarryingController _carryingController;

        public CarryingTowardsWeapon(NavMeshAgent navmeshAgent, Animator animator, RangedWeaponWithExternalAmmo weapon,
            CarryingController carryingController)
        {
            _navmeshAgent = navmeshAgent;
            _animator = animator;
            _weapon = weapon;
            _carryingController = carryingController;
        }

        public void Tick()
        {
            //Debug.Log($"IsCarrying: {_carryingController.IsCarrying} | Distance: {Vector3.Distance(_navmeshAgent.transform.position, _weapon.CarrierDropPoint.position)}");
        }

        public void OnEnter()
        {
            _animator.SetBool(WalkHash, true);
            _navmeshAgent.isStopped = false;
            _navmeshAgent.SetDestination(_weapon.CarrierDropPoint.position);
        }

        public void OnExit()
        {
            
        }
    }
}