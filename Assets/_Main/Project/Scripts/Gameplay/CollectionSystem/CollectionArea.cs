using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionSystem
{
    public class CollectionArea : MonoBehaviour
    {
        [SerializeField] private SplineComputer conveyorSpline;
        [SerializeField] private Transform destination;

        [SerializeField] private float conveyorHeightY = 0.5f;

        [SerializeField] private float approachAccel = 30f;
        [SerializeField] private float approachMaxSpeed = 10f;
        [SerializeField] private float approachStopDistance = 0.05f;

        [SerializeField] private float conveyorSpeed = 6f;
        [SerializeField] private float stopDistance = 0.05f;

        public void RegisterFragments(IEnumerable<GameObject> fragments)
        {
            foreach (var go in fragments.Where(f => f))
            {
                var frag = go.GetComponent<Fragment>() ?? go.AddComponent<Fragment>();
                frag.Initialize(
                    conveyorSpline,
                    conveyorHeightY,
                    approachAccel,
                    approachMaxSpeed,
                    approachStopDistance,
                    conveyorSpeed,
                    destination,
                    stopDistance
                );
                frag.StartTransportAsync().Forget();
            }
        }
    }
}