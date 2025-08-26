using UnityEngine;

namespace VisualEffects
{
    public class MaterialOffsetLooper : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 0.1f;
        [SerializeField] private float resetThreshold = -1f;

        private Material _material;
        private Vector2 _currentOffset;

        private static readonly int MainTexST = Shader.PropertyToID("_BaseMap_ST");

        private void Awake()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            _material = meshRenderer.material; // Instance oluşturur
            var st = _material.GetVector(MainTexST);
            _currentOffset = new Vector2(st.z, st.w); // offset x = z, y = w
        }

        private void Update()
        {
            _currentOffset.y -= scrollSpeed * Time.deltaTime;

            if (_currentOffset.y <= resetThreshold)
            {
                _currentOffset.y = 0f;
            }

            var st = new Vector4(1f, 1f, _currentOffset.x, _currentOffset.y); // tiling x,y = 1,1
            _material.SetVector(MainTexST, st);
        }
    }
}