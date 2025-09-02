using AI.Base.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace AI.CarrierAI
{
    public class WaitingWeaponToShoot : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private readonly NavMeshAgent _navmeshAgent;
        private readonly Animator _animator;

        public WaitingWeaponToShoot(NavMeshAgent navmeshAgent, Animator animator)
        {
            _navmeshAgent = navmeshAgent;
            _animator = animator;
        }

        public void Tick()
        {
            
        }
        
        public void OnEnter()
        {
            _navmeshAgent.isStopped = true;
            _animator.SetBool(WalkHash, false);
        }

        public void OnExit()
        {
        }
    }
}