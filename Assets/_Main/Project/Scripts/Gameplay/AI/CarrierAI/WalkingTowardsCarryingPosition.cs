using AI.Base.Interfaces;
using BasicStackSystem;
using UnityEngine;
using UnityEngine.AI;

namespace AI.CarrierAI
{
    public class WalkingTowardsCarryingPosition : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private readonly NavMeshAgent _navmeshAgent;
        private readonly Animator _animator;
        private readonly BasicStack _stack;

        public WalkingTowardsCarryingPosition(NavMeshAgent navmeshAgent, Animator animator, BasicStack stack)
        {
            _navmeshAgent = navmeshAgent;
            _animator = animator;
            _stack = stack;
        }

        public void Tick()
        {
            
        }

        public void OnEnter()
        {
            _navmeshAgent.isStopped = false;
            _navmeshAgent.SetDestination(_stack.transform.position);
            _animator.SetBool(WalkHash, true);
        }

        public void OnExit()
        {
            
        }
    }
}