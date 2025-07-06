using AI.Base.Interfaces;
using EventBusses;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Waiting : IState
    {
        private readonly Collider2D _connectedCollider;
        private readonly AIPath _aiPath;

        public Waiting(Collider2D connectedCollider, IEventBus eventBus, AIPath aiPath)
        {
            _connectedCollider = connectedCollider;
            _aiPath = aiPath;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            _connectedCollider.isTrigger = false;
            _aiPath.canMove = false;
        }

        public void OnExit()
        {
        }
    }
}