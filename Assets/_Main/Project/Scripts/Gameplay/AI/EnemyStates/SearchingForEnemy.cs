using AI.Base.Interfaces;
using Characters;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        private readonly Collider2D _connectedCollider;
        private readonly AIPath _aiPath;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly CharacterAnimationController _animationController;

        public SearchingForEnemy(Collider2D connectedCollider, AIPath aiPath, Rigidbody2D rigidbody2D,
            CharacterAnimationController animationController)
        {
            _connectedCollider = connectedCollider;
            _aiPath = aiPath;
            _rigidbody2D = rigidbody2D;
            _animationController = animationController;
        }

        public void Tick()
        {
            Debug.Log("Searching for enemy");
            _connectedCollider.isTrigger = false;
            _aiPath.canMove = false;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.linearVelocity = new Vector2();
        }

        public void OnEnter()
        {
            // _aiPath.canMove = false;
            // _rigidbody2D.angularVelocity = 0f;
            // _rigidbody2D.linearVelocity = new Vector2();
            _animationController.Idle();
        }

        public void OnExit()
        {
        }
    }
}