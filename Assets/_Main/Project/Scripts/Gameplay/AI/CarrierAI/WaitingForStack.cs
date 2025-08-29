using AI.Base.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace AI.CarrierAI
{
    public class WaitingForStack : IState
    {
        private readonly NavMeshAgent _navmeshAgent;
        private readonly Animator _animator;

        public WaitingForStack(NavMeshAgent navmeshAgent, Animator animator)
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
        }

        public void OnExit()
        {
        }
    }
}