using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using RayFire;
using UnityEngine;

namespace Characters.Enemy
{
    [Serializable]
    public class MeshColliderAndSkinnedMeshData
    {
        [SerializeField] public RayfireRigid rayfireRigid;
        [field: SerializeField] public GameObject ParentGameObjectOfColliders { get; private set; }
        public bool IsDestroyed { get; private set; }

        private MeshCollider[] _meshColliders;

        public void Initialize()
        {
            CollectComponentsAsync().Forget();
        }

        public void Demolish()
        {
            foreach (var meshCollider in _meshColliders)
            {
                meshCollider.enabled = false;
            }

            rayfireRigid.Demolish();
            IsDestroyed = true;
        }

        public bool CheckIfObjectIsPartOfParent(MeshCollider collider)
        {
            return _meshColliders.Contains(collider);
        }

        private async UniTask CollectComponentsAsync()
        {
            await UniTask.WaitForSeconds(1f);
            _meshColliders = ParentGameObjectOfColliders.GetComponentsInChildren<MeshCollider>();
        }
    }
}