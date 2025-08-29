using UnityEngine;
using System.Threading;
using PropertySystem;
using UnityEngine.AI;

namespace Characters.Enemy
{
    public class CharacterMovementController
    {
        private readonly Collider _collider;
        private readonly Rigidbody _rigidbody2D;
        private readonly CharacterAnimationController _animationController;
        private readonly Character _character;
        private readonly GameObject _model;
        private readonly PropertyData _speedProperty;
        private readonly NavMeshAgent _navmeshAgent;

        private CancellationTokenSource _rotationCTS;

        public CharacterMovementController(Collider collider, Rigidbody rigidbody2D,
            CharacterAnimationController animationController, Character character, GameObject model,
            PropertyData speedProperty, NavMeshAgent navmeshAgent)
        {
            _collider = collider;
            _rigidbody2D = rigidbody2D;
            _animationController = animationController;
            _character = character;
            _model = model;
            _speedProperty = speedProperty;
            _navmeshAgent = navmeshAgent;

            SetSpeedToDefault();
        }

        public bool GetIsReachedDistance(float checkPointDistance) => _navmeshAgent.remainingDistance <= checkPointDistance;

        public void StopCharacter(bool shouldActivatePhysics)
        {
            _navmeshAgent.destination = _character.transform.position;
            _animationController.Idle();
            _navmeshAgent.isStopped = true;

            StopRotater();
            SetPhysicsState(shouldActivatePhysics);
        }

        public void MoveCharacter(Vector3 pos, bool shouldActivatePhysics, float moveSpeed = 0)
        {
            _navmeshAgent.destination = pos;
            MoveCharacterInternal(shouldActivatePhysics, moveSpeed == 0 ? _speedProperty.TemporaryValue : moveSpeed);
        }

        private void MoveCharacterInternal(bool shouldActivatePhysics, float moveSpeed)
        {
            _animationController.Run();
            //_navmeshAgent.speed = moveSpeed;
            _navmeshAgent.isStopped = false;
            SetPhysicsState(shouldActivatePhysics);
        }
        

        public void StopRotater()
        {
            _rotationCTS?.Cancel();
            _rotationCTS?.Dispose();
            _rotationCTS = null;
        }
        
        public void IncreaseSpeedSmoothly(float smoothRate)
        {
            var newSpeed = Mathf.Lerp(_navmeshAgent.speed, _speedProperty.TemporaryValue, Time.deltaTime * smoothRate);
            _navmeshAgent.speed = newSpeed;
        }        
        
        public void SetSpeedToZero()
        {
            _navmeshAgent.speed = 0f;
        }        
        
        public void SetSpeedToDefault()
        {
            _navmeshAgent.speed = _speedProperty.TemporaryValue;
        }
        
        
        
        private void SetPhysicsState(bool shouldActivatePhysics)
        {
            _rigidbody2D.isKinematic = !shouldActivatePhysics;
            // _rigidbody2D.angularVelocity = new Vector3();
            // _rigidbody2D.linearVelocity = new Vector2();
            //Debug.Log($"PhysicsState: {shouldActivatePhysics}");      
            
            // if (shouldActivatePhysics)
            // {
            //     _rigidbody2D.bodyType = RigidbodyType2D.Dynamic; 
            //     _collider.isTrigger = false;
            // }
            // else
            // {
            //     _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            //     _collider.isTrigger = true;
            // }
        }
    }
}
