using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Characters.Enemy.CharacterDestructionSystem;
using CollectionField;
using Cysharp.Threading.Tasks;
using RayFire;
using UnityEngine;


namespace Characters.Enemy
{
    [Serializable]
    public class MeshColliderAndSkinnedMeshData
    {
        [field: SerializeField] public BodyPart BodyPart { get; private set; }
        [SerializeField] public RayfireRigid rayfireRigid;
        [field: SerializeField] public GameObject ParentGameObjectOfColliders { get; private set; }
        public bool IsDestroyed { get; private set; }


        private List<MeshCollider> _meshColliders = new();
        private CollectionArea _collectionArea;
        private CancellationTokenSource _cts;


        public void Initialize(CollectionArea collectionArea)
        {
            CollectComponentsAsync().Forget();
            _collectionArea = collectionArea;
        }


        public void Demolish()
        {
            if (IsDestroyed) return;


            foreach (var meshCollider in _meshColliders)
                meshCollider.enabled = false;


            rayfireRigid.Demolish();


            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var rbs = new List<Rigidbody>();
            foreach (var frag in rayfireRigid.fragments)
            {
                if (frag == null || frag.physics == null || frag.physics.rb == null)
                    continue;
                rbs.Add(frag.physics.rb);
            }


            if (rbs.Count > 0 && _collectionArea != null)
                _collectionArea.RegisterFragments(rbs, _cts.Token);

            IsDestroyed = true;
        }


        public bool CheckIfObjectIsPartOfParent(MeshCollider collider)
        {
            return _meshColliders.Contains(collider);
        }


        private async UniTask CollectComponentsAsync()
        {
            if (ParentGameObjectOfColliders == null) return;
            await UniTask.WaitForSeconds(1f);
            _meshColliders = ParentGameObjectOfColliders.GetComponentsInChildren<MeshCollider>().ToList();
        }
    }
}