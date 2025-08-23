using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionSystem
{
    public class Fragment : MonoBehaviour
    {
        private Rigidbody _rb;
        private Collider _col;
        private SplineFollower _follower;

        private SplineComputer _spline;
        private Transform _destination;

        private float _heightY;
        private float _approachMaxSpeed;
        private float _followSpeed;
        private float _stopDistance;
        private double _startPercent;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<Collider>();
            _follower = gameObject.AddComponent<SplineFollower>();
            _follower.follow = false;
        }

        public void Initialize(
            SplineComputer conveyorSpline,
            float conveyorHeightY,
            float approachMaxSpeed,
            float conveyorFollowSpeed,
            Transform destinationTransform,
            float destinationStopDist)
        {
            _spline = conveyorSpline;
            _heightY = conveyorHeightY;
            _approachMaxSpeed = approachMaxSpeed;
            _followSpeed = conveyorFollowSpeed;
            _destination = destinationTransform;
            _stopDistance = destinationStopDist;

            _follower.spline = _spline;

            var points = _spline.GetPoints();
            float y = points[0].position.y;
            var proj = _spline.Project(new Vector3(transform.position.x, y, transform.position.z));            
            _startPercent = proj.percent;
        }

        public async UniTaskVoid StartTransportAsync()
        {
            var beltPoint = _spline.Evaluate(_startPercent).position;
            var target = beltPoint;

            if (_col != null) _col.enabled = false;
            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.detectCollisions = false;
            }

            float distance = Vector3.Distance(transform.position, target);
            float duration = distance / _approachMaxSpeed;

            var seq =  DOTween.Sequence();
            seq.Append(transform.DOMove(target, duration).SetEase(Ease.InOutSine));
            seq.Join(transform.DORotate(Vector3.zero, duration));
            await seq.ToUniTask();

            _follower.applyDirectionRotation = false;
            _follower.SetPercent(_startPercent);
            _follower.followSpeed = _followSpeed;
            _follower.follow = true;
            
            while ((transform.position - _destination.position).sqrMagnitude > _stopDistance * _stopDistance)
            {
                await UniTask.Yield();
            }

            _follower.follow = false;
        }
    }
}
