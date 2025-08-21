using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RayFire;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyDestructionManager
    {
        private readonly List<MeshColliderAndSkinnedMeshData> _meshColliderAndSkinnedMeshDatas;
        private int _nextIndex;
        private System.Random _rng = new System.Random();

        public EnemyDestructionManager(List<MeshColliderAndSkinnedMeshData> meshColliderAndSkinnedMeshDatas)
        {
            _meshColliderAndSkinnedMeshDatas = meshColliderAndSkinnedMeshDatas;
            foreach (var meshColliderAndSkinnedMeshData in _meshColliderAndSkinnedMeshDatas)
            {
                meshColliderAndSkinnedMeshData.Initialize();
            }
        }

        public void DestroyPartIfPossible(GameObject hitObj)
        {
            if(hitObj == null) return;
            var collider = hitObj.GetComponent<MeshCollider>();
            var part = _meshColliderAndSkinnedMeshDatas.Find(i => i.CheckIfObjectIsPartOfParent(collider));
            
            if(part == null) return;
            
            part.Demolish();
        }

        public void DestroyAllParts()
        {
            foreach (var meshColliderAndSkinnedMeshData in _meshColliderAndSkinnedMeshDatas)
            {
                meshColliderAndSkinnedMeshData.Demolish();
            }
        }
        

        public MeshColliderAndSkinnedMeshData GetMeshColliderToAttack()
        {
            if (_meshColliderAndSkinnedMeshDatas == null || _meshColliderAndSkinnedMeshDatas.Count == 0)
                return null;

            // yok edilmemişleri filtrele
            var alive = _meshColliderAndSkinnedMeshDatas
                .Where(d => d != null && !d.IsDestroyed)
                .ToList();

            if (alive.Count == 0) 
                return null;

            // rastgele birini seç
            int idx = _rng.Next(alive.Count);
            return alive[idx];
        }


    }
}