using UnityEngine;
using Dreamteck.Splines;

public class RaySegmentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject raySegmentPrefab;
    [SerializeField] private SplineComputer spline;
    [SerializeField] private int segmentCount = 15;

    private void Start()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            var go = Instantiate(raySegmentPrefab, transform); // Parent olarak organize etmek için
            var deformer = go.GetComponent<RayDeformerOnSpline>();
            deformer.Initialize(spline, i, segmentCount);
        }
    }
}