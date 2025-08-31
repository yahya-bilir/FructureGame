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
        private readonly float _createdAmmoSplineSpeed;
        private PhysicsStack _stack;
        private AmmoCreator _ammoCreator;
        private Collider[] _colliders;

        public AmmoRailMovement(Transform connectedTransform, SplineComputer splineComputer,
            float createdAmmoSplineSpeed)
        {
            _connectedTransform = connectedTransform;
            _startPos = connectedTransform.position;
            _splineFollower = _connectedTransform.gameObject.AddComponent<SplineFollower>();
            _splineComputer = splineComputer;
            _createdAmmoSplineSpeed = createdAmmoSplineSpeed;
            GameObject = connectedTransform.gameObject;
            _colliders = GameObject.GetComponentsInChildren<Collider>();
            _rigidbody = connectedTransform.gameObject.GetComponent<Rigidbody>();
        }

        [Inject]
        private void Inject(PhysicsStack stack, AmmoCreator ammoCreator)
        {
            _stack = stack;
            _ammoCreator = ammoCreator;
        }
        
        [Button]
        private void AddVelocity()
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = true;
            }
            _rigidbody.isKinematic = false;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rigidbody.linearVelocity = -Vector3.right * 3;
        }

        [Button]
        private void ResetVelocity()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            //_connectedTransform.position = _startPos;
            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
        }

        public async UniTask InitiateMovementActions()
        {
            ResetVelocity();
            _splineFollower.spline = _splineComputer;
            _splineFollower.followSpeed = _createdAmmoSplineSpeed;
            _splineFollower.follow = true;
            
            while (_splineFollower.GetPercent() < 0.99)
            {
                await UniTask.Yield();
            }
            
            _splineFollower.follow = false;
            _splineFollower.enabled = false;
            GameObject.Destroy(_splineFollower);
            
            _stack.TryAddFromOutside(this);
            _ammoCreator.ReduceRequest();
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
            ResetVelocity();
        }
    }
}