using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionField
{
    public class CollectionArea : MonoBehaviour
    {
        public enum PlacementMode
        {
            // 1) Fizikle bant hacmine gir, sonra spline’a devret (XY çekiş, Z sabit)
            AttractXYThenSpline = 0,

            // 2) İlk frame’de bulunduğu Z’ye göre spline.percent hesapla, doğrudan o yüzdeye yerleş ve spline’a devret (Y hesabı yok)
            ProjectByZAndSnap = 1
        }


        [Header("References")] [SerializeField]
        private BoxCollider beltVolume;

        [SerializeField] private SplineComputer spline;

        [Tooltip("Varış (opsiyonel). Belirlenirse takip burada sonlandırılır.")] [SerializeField]
        private Transform finalTarget;


        [Header("Placement")] [SerializeField] private PlacementMode placementMode = PlacementMode.ProjectByZAndSnap;

        [Tooltip("Bantın sahnedeki sabit yüksekliği (Y)")] [SerializeField]
        private float beltY = 0.5f;


        [Header("Phase A: Physics (only for AttractXYThenSpline)")] [SerializeField]
        private float attractAccel = 30f;

        [SerializeField] private float attractMaxVel = 10f;
        [SerializeField] private float enterEps = 0.05f;


        [Header("Phase B: Spline (script-driven, no physics)")] [SerializeField]
        private float beltSpeed = 6f; // m/s benzeri hız; spline uzunluğuna bölünüp percent’e çevrilecek

        [SerializeField] private float stopEps = 0.05f;


        // Public API: Demolish sonrası rigidbody’leri kayda al
        public void RegisterFragments(IEnumerable<Rigidbody> rbs, CancellationToken token = default)
        {
            foreach (var rb in rbs.Where(r => r))
                HandleFragmentAsync(rb, token).Forget();
        }


        private async UniTaskVoid HandleFragmentAsync(Rigidbody rb, CancellationToken token)
        {
            if (rb == null) return;
            rb.WakeUp();


            float zLock = rb.position.z;


            if (placementMode == PlacementMode.AttractXYThenSpline)
            {
                await AttractIntoBeltAsync(rb, zLock, token);
                await SwitchToSplineControl(rb, zLock, token);
            }
            else // ProjectByZAndSnap
            {
                await SwitchToSplineControl(rb, zLock, token, snapImmediately: true);
            }
        }


        private async UniTask AttractIntoBeltAsync(Rigidbody rb, float zLock, CancellationToken token)
        {
            var b = beltVolume.bounds;


            while (!token.IsCancellationRequested)
            {
                Vector3 pos = rb.position;
                float tx = Mathf.Clamp(pos.x, b.min.x, b.max.x);
                float ty = Mathf.Clamp(pos.y, b.min.y, b.max.y);
                Vector3 target = new Vector3(tx, ty, zLock);
            }
        }

        private async UniTask SwitchToSplineControl(
            Rigidbody rb,
            float zLock,
            CancellationToken token,
            bool snapImmediately = false)
        {
            // 1) Başlangıç percent hesapla (bant yüksekliği sabit, Y = beltY)
            var startWorld = new Vector3(rb.position.x, beltY, zLock);
            var proj = spline.Project(startWorld);
            double t = proj.percent;

            foreach (var c in rb.GetComponentsInChildren<Collider>())
                c.enabled = false;
            rb.isKinematic = true;
            rb.detectCollisions = false;

            // 3) Spline uzunluğu al
            float splineLen = Mathf.Max(0.0001f, (float)spline.CalculateLength());

            if (snapImmediately)
            {
                var eval0 = spline.Evaluate(t);
                rb.transform.position = new Vector3(eval0.position.x, beltY, zLock);
                rb.transform.rotation = Quaternion.LookRotation(eval0.forward, Vector3.up);
                await UniTask.Yield(token);
            }

            while (!token.IsCancellationRequested)
            {
                t += (beltSpeed * Time.deltaTime) / splineLen;
                var eval = spline.Evaluate(t);

                rb.transform.position = new Vector3(eval.position.x, beltY, zLock);
                rb.transform.rotation = Quaternion.LookRotation(eval.forward, Vector3.up);

                if (finalTarget != null)
                {
                    float d2 = (rb.transform.position - finalTarget.position).sqrMagnitude;
                    if (d2 <= stopEps * stopEps) break;
                }

                await UniTask.Yield(token);
            }

            // Varış: burada objeyi pool’a iade veya pasifleştirebilirsin
        }
    }
}