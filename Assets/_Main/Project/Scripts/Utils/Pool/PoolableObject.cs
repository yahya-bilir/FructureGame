using UnityEngine;
using VContainer;

namespace Utils.Pool
{
    public class PoolableObject : MonoBehaviour
    {
        public string PoolTag { get; set; }
        [Inject] private PoolSystem _poolSystem;
        
        private void OnDisable()
        {
            transform.localScale = Vector3.one;
            _poolSystem.ReturnToPool(PoolTag, gameObject);
        }
    }
}