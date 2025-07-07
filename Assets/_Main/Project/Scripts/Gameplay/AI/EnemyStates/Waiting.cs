using AI.Base.Interfaces;
using Characters;
using EventBusses;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Waiting : IState
    {
        private readonly Collider2D _connectedCollider;
        private readonly AIPath _aiPath;
        private readonly CharacterAnimationController _animationController;

        public Waiting(Collider2D connectedCollider, IEventBus eventBus, AIPath aiPath,
            CharacterAnimationController animationController)
        {
            _connectedCollider = connectedCollider;
            _aiPath = aiPath;
            _animationController = animationController;
        }

        public void Tick()
        {
            Debug.Log("Waiting");
        }

        public void OnEnter()
        {
            _connectedCollider.isTrigger = false;
            _aiPath.canMove = false;
            _animationController.Idle();
        }

        public void OnExit()
        {
        }
    }
}