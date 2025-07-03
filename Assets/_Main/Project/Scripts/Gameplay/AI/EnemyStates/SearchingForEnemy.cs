using AI.Base.Interfaces;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        private readonly Collider2D _connectedCollider;

        public SearchingForEnemy(Collider2D connectedCollider)
        {
            _connectedCollider = connectedCollider;
        }

        public void Tick()
        {
            Debug.Log("Searching for enemy");
            _connectedCollider.isTrigger = false;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}