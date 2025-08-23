// CollectionArea.cs
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionSystem
{
    public class CollectionArea : MonoBehaviour
    {
        [SerializeField] private SplineComputer conveyorSpline;
        [SerializeField] private Transform destination;

        [SerializeField] private float conveyorHeightY = 0.5f;
        [SerializeField] private float approachMaxSpeed = 10f;
        [SerializeField] private float conveyorSpeed = 6f;
        [SerializeField] private float stopDistance = 0.05f;

        // Fragment'leri kayıt edip spline sistemine başlatır
        public async UniTask RegisterFragments(IEnumerable<GameObject> fragments)
        {
            await UniTask.WaitForSeconds(1.5f); // Gerekirse delay

            foreach (var go in fragments.Where(f => f))
            {
                var frag = go.GetComponent<Fragment>() ?? go.AddComponent<Fragment>();
                frag.Initialize(
                    conveyorSpline,
                    conveyorHeightY,
                    approachMaxSpeed,
                    conveyorSpeed,
                    destination,
                    stopDistance
                );
                frag.StartTransportAsync().Forget();
            }
        }
    }
}