using AI.Base.Interfaces;
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

        public CarryingTowardsWeapon(NavMeshAgent navmeshAgent, Animator animator, RangedWeaponWithExternalAmmo weapon)
        {
            _navmeshAgent = navmeshAgent;
            _animator = animator;
            _weapon = weapon;
        }

        public void Tick()
        {
            
        }

        public void OnEnter()
        {
            _animator.SetBool(WalkHash, true);
            _navmeshAgent.isStopped = false;
            _navmeshAgent.SetDestination(_weapon.transform.position);
        }

        public void OnExit()
        {
            
        }
    }
}