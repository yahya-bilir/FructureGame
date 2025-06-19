using Characters;
using DG.Tweening;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class ArrowAmmo : AmmoHomingBase
    {
        [SerializeField] private float jumpPower = 1f;  // yay yüksekliği
        [SerializeField] private int jumpCount = 1;

        protected override void PlayTween(Character target)
        {
            Vector3 localTargetPos = target.transform.InverseTransformPoint(target.transform.position);
            float distance = Vector3.Distance(transform.localPosition, localTargetPos);
            float duration = distance / _speed;

            float angle = Mathf.Atan2(localTargetPos.y - transform.localPosition.y, localTargetPos.x - transform.localPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            transform.parent = target.transform;
            
            _moveTween?.Kill();
            _moveTween = transform.DOLocalJump(Vector3.zero, jumpPower, jumpCount, duration, snapping: false)
                .SetEase(Ease.OutQuad)
                .SetRelative(false)
                .OnComplete(OnTweenComplete);
        }
    }
}