using AI.Base.Interfaces;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Waiting : IState
    {
        private readonly Collider2D _connectedCollider;

        public Waiting(Collider2D connectedCollider)
        {
            _connectedCollider = connectedCollider;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            _connectedCollider.isTrigger = false;
        }

        public void OnExit()
        {
        }
    }
}