using AI.Base.Interfaces;
using BasicStackSystem;
using Characters.CarrierAI;
using UnityEngine;
using UnityEngine.AI;

namespace AI.CarrierAI
{
    public class WalkingTowardsCarryingPosition : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private readonly NavMeshAgent _navmeshAgent;
        private readonly Animator _animator;
        private readonly CarryingController _carryingController;
        private readonly CarrierAIBehaviour _carrierAIBehaviour;

        public WalkingTowardsCarryingPosition(NavMeshAgent navmeshAgent, Animator animator,
            CarryingController carryingController, CarrierAIBehaviour carrierAIBehaviour)
        {
            _navmeshAgent = navmeshAgent;
            _animator = animator;
            _carryingController = carryingController;
            _carrierAIBehaviour = carrierAIBehaviour;
        }

        public void Tick()
        {
            
        }

        public void OnEnter()
        {
            _carrierAIBehaviour.ClosestPosition = _carryingController.GetClosestPosition();
            _navmeshAgent.isStopped = false;
            _navmeshAgent.SetDestination(_carrierAIBehaviour.ClosestPosition);
            _animator.SetBool(WalkHash, true);
        }

        public void OnExit()
        {
            
        }
    }
}