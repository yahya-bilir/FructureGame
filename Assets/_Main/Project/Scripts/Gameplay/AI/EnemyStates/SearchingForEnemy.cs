using AI.Base.Interfaces;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        private readonly Collider2D _connectedCollider;
        private readonly AIPath _aiPath;

        public SearchingForEnemy(Collider2D connectedCollider, AIPath aiPath)
        {
            _connectedCollider = connectedCollider;
            _aiPath = aiPath;
        }

        public void Tick()
        {
            Debug.Log("Searching for enemy");
            _connectedCollider.isTrigger = false;
        }

        public void OnEnter()
        {
            _aiPath.canMove = false;
        }

        public void OnExit()
        {
        }
    }
}