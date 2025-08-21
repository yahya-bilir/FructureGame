using System;
using System.Collections.Generic;
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

        private List<MeshCollider> _meshColliders = new();

        public void Initialize()
        {
            CollectComponentsAsync().Forget();
        }

        public void Demolish()
        {
            foreach (var meshCollider in _meshColliders)
                meshCollider.enabled = false;

            rayfireRigid.Demolish();

            // // parçaları kendi -forward yönlerine doğru it
            // const float force = 3;               // genel itme şiddeti (impulse)
            // const float spread = 0.1f;            // yönü biraz dağıtmak için
            // const float upwardBias = 0f;        // hafif yukarı bileşen (isteğe bağlı)
            //
            // foreach (var fragment in rayfireRigid.fragments)
            // {
            //     if (fragment == null || fragment.physics == null || fragment.physics.rb == null)
            //         continue;
            //
            //     var rb = fragment.physics.rb;
            //
            //     // her parçanın kendi arkası
            //     Vector3 backDir = -fragment.transform.forward;
            //
            //     // biraz rastgelelik + hafif yukarı bileşen
            //     Vector3 jitter = UnityEngine.Random.insideUnitSphere * spread + Vector3.up * upwardBias;
            //
            //     // yönü normalize et, impulse uygula
            //     Vector3 dir = (backDir + jitter).normalized;
            //     rb.AddForce(dir * force, ForceMode.Impulse);
            //
            //     // opsiyonel: hafif dönüş için tork ver
            //     rb.AddTorque(UnityEngine.Random.insideUnitSphere * force * 0.25f, ForceMode.Impulse);
            // }

            DestroyPartAfterInterval().Forget();
            IsDestroyed = true;
        }


        public bool CheckIfObjectIsPartOfParent(MeshCollider collider)
        {
            return _meshColliders.Contains(collider);
        }

        private async UniTask CollectComponentsAsync()
        {
            if(ParentGameObjectOfColliders == null) return;
            await UniTask.WaitForSeconds(1f);
            _meshColliders = ParentGameObjectOfColliders.GetComponentsInChildren<MeshCollider>().ToList();
        }
        
        private async UniTask DestroyPartAfterInterval()
        {
            await UniTask.WaitForSeconds(1.5f);
            foreach (var rigidFragment in rayfireRigid.fragments)
            {
                rigidFragment.gameObject.SetActive(false);
            }
        }
    }
}