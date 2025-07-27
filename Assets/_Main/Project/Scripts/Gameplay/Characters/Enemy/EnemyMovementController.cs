using UnityEngine;
using System.Threading;
using PropertySystem;
using UnityEngine.AI;

namespace Characters.Enemy
{
    public class EnemyMovementController
    {
        private readonly Collider2D _collider;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly CharacterAnimationController _animationController;
        private readonly EnemyBehaviour _enemyBehaviour;
        private readonly GameObject _model;
        private readonly PropertyData _speedProperty;
        private readonly NavMeshAgent _navmeshAgent;

        private CancellationTokenSource _rotationCTS;

        public EnemyMovementController(Collider2D collider, Rigidbody2D rigidbody2D,
            CharacterAnimationController animationController, EnemyBehaviour enemyBehaviour, GameObject model,
            PropertyData speedProperty, NavMeshAgent navmeshAgent)
        {
            _collider = collider;
            _rigidbody2D = rigidbody2D;
            _animationController = animationController;
            _enemyBehaviour = enemyBehaviour;
            _model = model;
            _speedProperty = speedProperty;
            _navmeshAgent = navmeshAgent;
        }

        public bool GetIsReachedDistance(float checkPointDistance) => _navmeshAgent.remainingDistance <= checkPointDistance;

        public void StopCharacter(bool shouldActivatePhysics)
        {
            _navmeshAgent.destination = _enemyBehaviour.transform.position;
            _animationController.Idle();
            _navmeshAgent.isStopped = true;

            StopRotater();
            SetPhysicsState(shouldActivatePhysics);
        }

        public void MoveCharacter(Vector2 pos, bool shouldActivatePhysics, float moveSpeed)
        {
            _navmeshAgent.destination = pos;
            MoveCharacterInternal(shouldActivatePhysics, moveSpeed);
        }

        private void MoveCharacterInternal(bool shouldActivatePhysics, float moveSpeed)
        {
            _animationController.Run();
            _navmeshAgent.speed = moveSpeed;
            _navmeshAgent.isStopped = false;
            SetPhysicsState(shouldActivatePhysics);
        }
        

        public void StopRotater()
        {
            _rotationCTS?.Cancel();
            _rotationCTS?.Dispose();
            _rotationCTS = null;
        }
        

        private void SetPhysicsState(bool shouldActivatePhysics)
        {
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.linearVelocity = new Vector2();
            Debug.Log($"PhysicsState: {shouldActivatePhysics}");      
            
            if (shouldActivatePhysics)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Dynamic; 
                _collider.isTrigger = false;
            }
            else
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _collider.isTrigger = true;
            }
        }
    }
}
