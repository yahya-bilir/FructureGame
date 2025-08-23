using System;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionSystem
{
    public class Fragment : MonoBehaviour
    {
        private Rigidbody _rb;
        private SplineFollower _follower;

        private SplineComputer _spline;
        private Transform _destination;

        private float _heightY;
        private float _approachAccel;
        private float _approachMaxSpeed;
        private float _approachStopDistance;
        private float _followSpeed;
        private float _stopDistance;

        private double _startPercent;

        private void Awake()
        {
            _follower = gameObject.AddComponent<SplineFollower>();
            _follower.follow = false;
            _rb = GetComponent<Rigidbody>();
        }
        

        public void Initialize(
            SplineComputer conveyorSpline,
            float conveyorHeightY,
            float approachAcceleration,
            float approachMaxVel,
            float approachStopDist,
            float conveyorFollowSpeed,
            Transform destinationTransform,
            float destinationStopDist)
        {
            _spline = conveyorSpline;
            _heightY = conveyorHeightY;
            _approachAccel = approachAcceleration;
            _approachMaxSpeed = approachMaxVel;
            _approachStopDistance = approachStopDist;
            _followSpeed = conveyorFollowSpeed;
            _destination = destinationTransform;
            _stopDistance = destinationStopDist;

            _follower.spline = _spline;

            float z = transform.position.z;
            var proj = _spline.Project(new Vector3(transform.position.x, _heightY, z));
            _startPercent = proj.percent;
        }

        public async UniTaskVoid StartTransportAsync()
        {
            float z = _rb.position.z;
            var beltPoint = _spline.Evaluate(_startPercent).position;
            var target = new Vector3(beltPoint.x, _heightY, z);

            var delta = new Vector2(target.x - _rb.position.x, target.y - _rb.position.y);

            while (delta.sqrMagnitude > _approachStopDistance * _approachStopDistance)
            {
                Debug.Log($"[Fragment] Conveyor'a yaklaşma mesafesi: {delta.magnitude}");

                var velXY = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y);
                var desired = delta.normalized * _approachMaxSpeed;
                var need = desired - velXY;

                var accel = need.normalized * _approachAccel;
                _rb.AddForce(new Vector3(accel.x, accel.y, 0f), ForceMode.Acceleration);
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, 0f);

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                // burada delta'yı güncelliyoruz
                delta = new Vector2(target.x - _rb.position.x, target.y - _rb.position.y);
            }

            var col = GetComponent<Collider>();
            if(col != null)col.enabled = false;
            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.detectCollisions = false;
            }
            
            _follower.SetPercent(_startPercent);
            _follower.followSpeed = _followSpeed;
            _follower.follow = true;
            
            while (true)
            {
                var d2 = (transform.position - _destination.position).sqrMagnitude;
                if (d2 <= _stopDistance * _stopDistance) break;
                await UniTask.Yield();
            }

            _follower.follow = false;
        }
    }
}
