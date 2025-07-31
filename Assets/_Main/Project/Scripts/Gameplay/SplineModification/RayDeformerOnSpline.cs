using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(MeshFilter))]
public class RayDeformerOnSpline : MonoBehaviour
{
    [SerializeField] private SplineComputer spline;

    private int index;
    private int totalCount;

    public void Initialize(SplineComputer spline, int index, int totalCount)
    {
        this.spline = spline;
        this.index = index;
        this.totalCount = totalCount;

        Deform();
    }

    private void Deform()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh originalMesh = meshFilter.sharedMesh;

        if (originalMesh == null)
        {
            Debug.LogError("Mesh yok.");
            return;
        }

        Mesh deformedMesh = Instantiate(originalMesh);
        Vector3[] originalVerts = originalMesh.vertices;
        Vector3[] deformedVerts = new Vector3[originalVerts.Length];

        float startPercent = (float)index / totalCount;
        float endPercent = (float)(index + 1) / totalCount;

        for (int i = 0; i < originalVerts.Length; i++)
        {
            float t = Mathf.InverseLerp(-0.5f, 0.5f, originalVerts[i].z);
            float percent = Mathf.Lerp(startPercent, endPercent, t);

            SplineSample sample = new SplineSample();
            spline.Evaluate(percent, ref sample);

            Vector3 localPos = originalVerts[i];
            Vector3 offset = sample.right * localPos.x + sample.up * localPos.y;
            deformedVerts[i] = sample.position + offset;
        }

        deformedMesh.vertices = deformedVerts;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.uv = originalMesh.uv;
        deformedMesh.RecalculateNormals();

        meshFilter.mesh = deformedMesh;
    }
}