using AI.Base.Interfaces;
using Characters.CarrierAI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace AI.CarrierAI
{
    public class DroppingAmmo : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private readonly CarryingController _carryingController;
        private readonly Animator _animator;
        private readonly NavMeshAgent _navmeshAgent;

        public DroppingAmmo(CarryingController carryingController, Animator animator, NavMeshAgent navmeshAgent)
        {
            _carryingController = carryingController;
            _animator = animator;
            _navmeshAgent = navmeshAgent;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            _navmeshAgent.isStopped = true;
            _animator.SetBool(WalkHash, false);
            _carryingController.Drop().Forget();
        }

        public void OnExit()
        {
        }
    }
}