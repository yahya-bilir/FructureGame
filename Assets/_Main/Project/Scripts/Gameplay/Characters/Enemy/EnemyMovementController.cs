using Cysharp.Threading.Tasks;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;
using System.Threading;
using PropertySystem;

namespace Characters.Enemy
{
    public class EnemyMovementController
    {
        private readonly AIPath _aiPath;
        private readonly RVOController _rvoController;
        private readonly AIDestinationSetter _aiDestinationSetter;
        private readonly Collider2D _collider;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly CharacterAnimationController _animationController;
        private readonly EnemyBehaviour _enemyBehaviour;
        private readonly GameObject _model;
        private readonly PropertyData _speedProperty;

        private CancellationTokenSource _rotationCTS;

        public EnemyMovementController(AIPath aiPath, RVOController rvoController,
            AIDestinationSetter aiDestinationSetter, Collider2D collider, Rigidbody2D rigidbody2D,
            CharacterAnimationController animationController, EnemyBehaviour enemyBehaviour, GameObject model,
            PropertyData speedProperty)
        {
            _aiPath = aiPath;
            _rvoController = rvoController;
            _aiDestinationSetter = aiDestinationSetter;
            _collider = collider;
            _rigidbody2D = rigidbody2D;
            _animationController = animationController;
            _enemyBehaviour = enemyBehaviour;
            _model = model;
            _speedProperty = speedProperty;
        }

        public bool GetIsReachedDistance(float checkPointDistance) => _aiPath.remainingDistance <= checkPointDistance;

        public void StopCharacter(bool shouldActivatePhysics)
        {
            _aiPath.destination = _enemyBehaviour.transform.position;
            _aiDestinationSetter.target = null;
            _aiPath.FinalizeMovement(_enemyBehaviour.transform.position, Quaternion.identity);
            _animationController.Idle();
            _aiPath.canMove = false;

            StopRotater();
            SetPhysicsState(shouldActivatePhysics);
        }

        public void MoveCharacter(Vector2 pos, bool shouldActivatePhysics, float moveSpeed)
        {
            _aiPath.destination = pos;
            _aiDestinationSetter.target = null;

            MoveCharacterInternal(shouldActivatePhysics, moveSpeed);
        }

        public void MoveCharacter(Transform pos, bool shouldActivatePhysics, float moveSpeed)
        {
            _aiDestinationSetter.target = pos;

            MoveCharacterInternal(shouldActivatePhysics, moveSpeed);
        }

        private void MoveCharacterInternal(bool shouldActivatePhysics, float moveSpeed)
        {
            _animationController.Run();
            _aiPath.maxSpeed = moveSpeed;
            _aiPath.canMove = true;

            StartRotater();
            SetPhysicsState(shouldActivatePhysics);
        }

        public void ToggleRVO(bool shouldActivate) => _rvoController.enabled = shouldActivate;

        public void StartRotater()
        {
            StopRotater(); // varsa önceki iptal edilir
            _rotationCTS = new CancellationTokenSource();
            MovementRotater(_rotationCTS.Token).Forget();
        }

        public void StopRotater()
        {
            _rotationCTS?.Cancel();
            _rotationCTS?.Dispose();
            _rotationCTS = null;
        }

        private async UniTask MovementRotater(CancellationToken cancellationToken)
        {
            var modelTransform = _model.transform;
            while (!cancellationToken.IsCancellationRequested)
            {
                var velocity = _aiPath.desiredVelocity;
                if (velocity.x > 0.1f)
                {
                    modelTransform.localEulerAngles = new Vector3(0, 0, 0);
                }
                else if (velocity.x < -0.1f)
                {
                    modelTransform.localEulerAngles = new Vector3(0, 180, 0);
                }

                await UniTask.Yield();
            }
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
