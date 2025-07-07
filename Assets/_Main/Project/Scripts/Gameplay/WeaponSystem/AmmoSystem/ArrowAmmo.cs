using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class ArrowAmmo : AmmoHomingBase
    {
        [SerializeField] private Transform rotationTarget;

        [Header("Arc Angles")]
        [SerializeField] private float startAngle = 45f;
        [SerializeField] private float apexAngle = 90f;
        [SerializeField] private float endAngle = 0f;
        [SerializeField] private float apexTime = 0.5f;

        [Header("Arc Settings")]
        [SerializeField] private float arcFactor = 0.2f;    // Arc yüksekliği yatay mesafeye oran
        [SerializeField] private float minArcHeight = 0.5f; // Arc kaybolmasın

        protected override async void PlayTween(Character target)
        {
            transform.SetParent(target.transform, true);

            Vector3 startLocalPos = transform.localPosition;
            Vector3 endLocalPos = target.transform.InverseTransformPoint(target.transform.position);

            // Ana yön vektörü
            Vector3 dir = endLocalPos - startLocalPos;

            // Hedefe bakan base rotation
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(dir.y) < 0.01f) baseAngle = 0f;
            transform.localRotation = Quaternion.Euler(0, 0, baseAngle);

            // Flip: sola gidiyorsa sprite aynalanır
            bool isGoingLeft = dir.x < 0f;
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Abs(scale.y) * (isGoingLeft ? -1f : 1f);
            transform.localScale = scale;

            // Yatay mesafe hesapla
            float horizontalDistance = Mathf.Abs(endLocalPos.x - startLocalPos.x);
            if (horizontalDistance < 0.1f)
            {
                float verticalDistance = Mathf.Abs(endLocalPos.y - startLocalPos.y);
                horizontalDistance = verticalDistance * 0.5f;
            }

            float arcHeight = Mathf.Max(minArcHeight, horizontalDistance * arcFactor);

            float traveled = 0f;

            if (rotationTarget != null)
                rotationTarget.localRotation = Quaternion.Euler(0, 0, startAngle);

            while (traveled < horizontalDistance)
            {
                float step = _speed * Time.deltaTime;
                traveled += step;

                float t = Mathf.Clamp01(traveled / horizontalDistance);

                float newX = Mathf.Lerp(startLocalPos.x, endLocalPos.x, t);
                float newY = Mathf.Lerp(startLocalPos.y, endLocalPos.y, t);

                float height = 4f * arcHeight * t * (1 - t);
                newY += height;

                transform.localPosition = new Vector3(newX, newY, startLocalPos.z);

                float angle;
                if (t <= apexTime)
                {
                    float upT = t / apexTime;
                    angle = Mathf.Lerp(startAngle, apexAngle, upT);
                }
                else
                {
                    float downT = (t - apexTime) / (1 - apexTime);
                    angle = Mathf.Lerp(apexAngle, endAngle, downT);
                }

                if (rotationTarget != null)
                {
                    rotationTarget.localRotation = Quaternion.Euler(0, 0, angle);
                }

                await UniTask.Yield();
            }

            OnTweenComplete();
        }
    }
}
