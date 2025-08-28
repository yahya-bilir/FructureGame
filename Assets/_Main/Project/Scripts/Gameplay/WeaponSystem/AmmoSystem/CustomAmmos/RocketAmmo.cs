using System;
using System.Threading;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem.CustomAmmos
{
    public class RocketAmmo : AmmoAOEProjectile
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float arcHeight = 2f;
        [SerializeField] private float selfDestructTime = 3f;

        private Vector3 _targetPos;
        private CancellationTokenSource _cts;

        protected override void FireAtPosition(Vector3 targetPos)
        {
            _targetPos = targetPos;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _hasExploded = false;

            _rigidbody.useGravity = false;

            StartArcTrajectory(_cts.Token).Forget();
            AutoDestruct(_cts.Token).Forget();
        }

        private async UniTaskVoid StartArcTrajectory(CancellationToken token)
        {
            Vector3 start = transform.position;
            Vector3 end = _targetPos;
            float distance = Vector3.Distance(start, end);
            float totalTime = distance / speed;

            float elapsed = 0f;
            while (!_hasExploded && elapsed < totalTime)
            {
                token.ThrowIfCancellationRequested();
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / totalTime);

                Vector3 linear = Vector3.Lerp(start, end, t);
                float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
                linear.y += arc;

                Vector3 nextPos = linear;
                Vector3 direction = (nextPos - transform.position).normalized;

                _rigidbody.linearVelocity = direction * speed;
                transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 10f);

                await UniTask.Yield();
            }

            Explode(); // çarpmadan da gidebilir
        }

        private async UniTaskVoid AutoDestruct(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(selfDestructTime), cancellationToken: token);
                if (!_hasExploded)
                    Explode();
            }
            catch (OperationCanceledException) { }
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering)
        {
            if (!isEntering || _hasExploded) return;
            if (!other.CompareTag("Ground")) return;
            if (!other.TryGetComponent(out Character character)) return;
            if (character == ConnectedCombatManager.Character) return;

            Explode();
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}
