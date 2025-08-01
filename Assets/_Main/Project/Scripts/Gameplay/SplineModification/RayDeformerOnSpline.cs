using UnityEngine;
using Dreamteck.Splines;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class RayDeformerOnSpline : MonoBehaviour
{
    private SplineComputer _spline;
    private int _index;
    private int _totalCount;
    private Vector2 _scale;
    private MeshFilter _meshFilter;
    private Transform _meshTransform;

    [SerializeField] private ParticleSystem spawnVFX;

    public async UniTask Initialize(SplineComputer spline, int index, int totalCount, Vector2 scale)
    {
        _spline = spline;
        _index = index;
        _totalCount = totalCount;
        _scale = scale;

        _meshFilter = GetComponentInChildren<MeshFilter>();
        if (_meshFilter == null)
        {
            Debug.LogError("RayDeformerOnSpline: Child MeshFilter bulunamadı.");
            return;
        }

        _meshTransform = _meshFilter.transform;

        float startPercent = (float)_index / _totalCount;
        float endPercent = (float)(_index + 1) / _totalCount;
        float midPercent = (startPercent + endPercent) / 2f;

        var sample = new SplineSample();
        _spline.Evaluate(midPercent, ref sample);

        transform.rotation = Quaternion.LookRotation(sample.forward, sample.up);


        // Yükseltilmiş pozisyona al
        _meshFilter.gameObject.SetActive(false);
        transform.position = sample.position /*+ Vector3.up * 3f*/;
        Deform();
        
        // Bekle, sonra düşür
        transform.position = sample.position + Vector3.up * 3f;
        _meshFilter.gameObject.SetActive(true);

        //await UniTask.WaitForSeconds(0.15f);
        await transform.DOMove(sample.position, 0.2f).SetEase(Ease.OutBounce).ToUniTask();

        // Önce mesh'i deforme et
        if (spawnVFX != null)
        {
            spawnVFX.Play();
        }
    }

    private void Deform()
    {
        var originalMesh = _meshFilter.sharedMesh;
        if (originalMesh == null)
        {
            Debug.LogError("RayDeformerOnSpline: Mesh bulunamadı.");
            return;
        }

        var deformedMesh = Instantiate(originalMesh);
        var originalVerts = originalMesh.vertices;
        var deformedVerts = new Vector3[originalVerts.Length];

        float startPercent = (float)_index / _totalCount;
        float endPercent = (float)(_index + 1) / _totalCount;

        for (int i = 0; i < originalVerts.Length; i++)
        {
            float t = Mathf.InverseLerp(-0.5f, 0.5f, originalVerts[i].z);
            float percent = Mathf.Lerp(startPercent, endPercent, t);

            var sample = new SplineSample();
            _spline.Evaluate(percent, ref sample);

            Vector3 local = originalVerts[i];
            local.x *= _scale.x;
            local.y *= _scale.y;

            Vector3 offset = sample.right * local.x + sample.up * local.y;
            deformedVerts[i] = _meshTransform.InverseTransformPoint(sample.position + offset);
        }

        deformedMesh.vertices = deformedVerts;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.uv = originalMesh.uv;
        deformedMesh.RecalculateNormals();

        _meshFilter.mesh = deformedMesh;
    }
}
