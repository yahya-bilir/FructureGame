using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CollectionSystem
{
    public class AmmoRailMovement : MonoBehaviour
    {
        [SerializeField] private float forceAmount;
        private SplineFollower _splineFollower;
        
        private Rigidbody _rigidbody;
        private Vector3 _startPos;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _startPos = transform.position;
            _splineFollower = gameObject.AddComponent<SplineFollower>();
        }

        [Button]
        private void AddVelocity()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = -Vector3.right * forceAmount;
        }

        [Button]
        private void ResetVelocity()
        {
            _rigidbody.isKinematic = true;
            
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.position = _startPos;
        }

        public async UniTask InitiateMovementActions(SplineComputer splineComputer)
        {
            ResetVelocity();
            _splineFollower.spline = splineComputer;
            _splineFollower.follow = true;
            
            while (_splineFollower.GetPercent() < 0.99)
            {
                await UniTask.Yield();
            }
        }
    }
}