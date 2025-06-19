using Characters;
using DG.Tweening;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class DirectAmmo : AmmoHomingBase
    {
        protected override void PlayTween(Character target)
        {
            Vector3 localTargetPos = target.transform.InverseTransformPoint(target.transform.position);
            float distance = Vector3.Distance(transform.localPosition, localTargetPos);
            float duration = distance / _speed;

            float angle = Mathf.Atan2(localTargetPos.y - transform.localPosition.y, localTargetPos.x - transform.localPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            _moveTween?.Kill();
            _moveTween = transform.DOLocalMove(localTargetPos, duration)
                .SetEase(Ease.Linear)
                .OnComplete(OnTweenComplete);
        }
    }
}