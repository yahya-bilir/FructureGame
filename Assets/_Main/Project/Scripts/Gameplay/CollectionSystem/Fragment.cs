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

        private float _approachMaxSpeed;
        private float _followSpeed;
        private double _startPercent;
        private CollectionArea _collectionArea;
        private MeshRenderer _meshRenderer;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _follower = gameObject.AddComponent<SplineFollower>();
            _follower.follow = false;
        }

        public void Initialize(SplineComputer conveyorSpline,
            float approachMaxSpeed,
            float conveyorFollowSpeed, 
            CollectionArea collectionArea)
        {
            _spline = conveyorSpline;
            _approachMaxSpeed = approachMaxSpeed;
            _followSpeed = conveyorFollowSpeed;
            _collectionArea = collectionArea;
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
            
            while (_follower.GetPercent() < 0.99)
            {
                await UniTask.Yield();
            }

            _meshRenderer.enabled = false;
            _follower.follow = false;
            _collectionArea.AddDeployedFragment(this);
        }
    }
}
