using UnityEngine;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class RaySegmentSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject raySegmentPrefab;
    [SerializeField] private SplineComputer spline;
    [SerializeField] private int segmentCount = 15;
    [SerializeField] private Vector2 segmentScale = new Vector2(0.33f, 0.5f);
    [SerializeField] private float totalSpawnTime = 1.5f;

    // Editörde görülecek buton buraya taşındı
    [Button("Ray Meshlerini Güncelle")]
    private void CallSpawnSegments()
    {
        SpawnSegments().Forget(); // isteğe bağlı olarak .Forget() burada kalabilir
    }

    public async UniTask SpawnSegments()
    {
        if (spline == null || raySegmentPrefab == null)
        {
            Debug.LogError("RaySegmentSpawner: Eksik referans.");
            return;
        }

        // Tüm eski çocukları sil
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        float perSegmentDelay = totalSpawnTime / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(raySegmentPrefab, transform);
            var deformer = segment.GetComponent<RayDeformerOnSpline>();

            if (deformer != null)
            {
                deformer.Initialize(spline, i, segmentCount, segmentScale).Forget(); // fire-and-forget, await yok
            }
            else
            {
                Debug.LogError("RaySegmentSpawner: RayDeformerOnSpline component eksik.");
            }

            await UniTask.WaitForSeconds(perSegmentDelay);
        }
    }
}