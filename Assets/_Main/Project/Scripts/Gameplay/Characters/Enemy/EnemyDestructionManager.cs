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
        private readonly List<RayfireRigid> _restOfTheRigids;
        private int _nextIndex;
        private System.Random _rng = new System.Random();

        public EnemyDestructionManager(List<MeshColliderAndSkinnedMeshData> meshColliderAndSkinnedMeshDatas,
            List<RayfireRigid> restOfTheRigids)
        {
            _meshColliderAndSkinnedMeshDatas = meshColliderAndSkinnedMeshDatas;
            _restOfTheRigids = restOfTheRigids;
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
            DestroyPartAfterInterval(part.rayfireRigid).Forget();
        }

        public void DestroyAllParts()
        {
            foreach (var meshColliderAndSkinnedMeshData in _meshColliderAndSkinnedMeshDatas)
            {
                meshColliderAndSkinnedMeshData.Demolish();
                DestroyPartAfterInterval(meshColliderAndSkinnedMeshData.rayfireRigid).Forget();

            }

            foreach (var rigid in _restOfTheRigids)
            {
                rigid.Demolish();
                DestroyPartAfterInterval(rigid).Forget();
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

        private async UniTask DestroyPartAfterInterval(RayfireRigid rigid)
        {
            await UniTask.WaitForSeconds(1.5f);
            foreach (var rigidFragment in rigid.fragments)
            {
                rigidFragment.gameObject.SetActive(false);
            }
        }
    }
}