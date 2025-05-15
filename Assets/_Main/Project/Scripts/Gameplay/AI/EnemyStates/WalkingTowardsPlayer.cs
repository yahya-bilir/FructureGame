using AI.Base.Interfaces;
using Characters.Enemy;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class WalkingTowardsPlayer : IState
    {
        private readonly EnemyAnimationController _animationController;
        private readonly Transform _playerTransform;
        private readonly AIPath _aiPath;
        private readonly Transform _modelTransform;

        public WalkingTowardsPlayer(EnemyAnimationController animationController, Transform playerTransform,
            AIPath aiPath, Transform model)
        {
            _animationController = animationController;
            _playerTransform = playerTransform;
            _aiPath = aiPath;
            _modelTransform = model;
        }

        public void Tick()
        {
            Vector3 velocity = _aiPath.desiredVelocity;

            if (velocity.x > 0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 0, 0); // Sağa bak
            }
            else if (velocity.x < -0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 180, 0); // Sola bak
            }

            Debug.Log(_aiPath.remainingDistance);
            
            Debug.Log("Walking");
        }

        public void OnEnter()
        {
            _animationController.Run();
            _aiPath.canMove = true;
        }

        public void OnExit()
        {
            _aiPath.canMove = false;
        }
    }
}