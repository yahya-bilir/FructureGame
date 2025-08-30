using BasicStackSystem;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Vector3 = UnityEngine.Vector3;

namespace CollectionSystem
{
    public class AmmoRailMovement : IStackable
    {
        private SplineFollower _splineFollower;
        private Transform _connectedTransform;
        private Rigidbody _rigidbody;
        private Vector3 _startPos;
        private SplineComputer _splineComputer;
        private PhysicsStack _stack;

        public AmmoRailMovement(Transform connectedTransform, Rigidbody rigidbody, SplineComputer splineComputer)
        {
            _rigidbody = rigidbody;
            _connectedTransform = connectedTransform;
            _startPos = connectedTransform.position;
            _splineFollower = _connectedTransform.gameObject.AddComponent<SplineFollower>();
            _splineComputer = splineComputer;
            GameObject = connectedTransform.gameObject;
        }

        [Inject]
        private void Inject(PhysicsStack stack)
        {
            _stack = stack;
        }
        
        [Button]
        private void AddVelocity()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = -Vector3.right * 3;
        }

        [Button]
        private void ResetVelocity()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            //_connectedTransform.position = _startPos;
        }

        public async UniTask InitiateMovementActions()
        {
            ResetVelocity();
            _splineFollower.spline = _splineComputer;
            _splineFollower.followSpeed = 3.25f;
            _splineFollower.follow = true;
            
            while (_splineFollower.GetPercent() < 0.99)
            {
                await UniTask.Yield();
            }
            
            _splineFollower.follow = false;
            _stack.TryAddFromOutside(this);
            AddVelocity();

        }

        public GameObject GameObject { get; private set; }
        public void OnObjectStartedBeingCarried()
        {
        }

        public void OnObjectCollected()
        {
        }

        public void OnObjectDropped()
        {
        }
    }
}