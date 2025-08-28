using System.Collections.Generic;
using System.Linq;
using Characters.Enemy.CharacterDestructionSystem;
using CollectionSystem;
using UnityEngine;
using VContainer;

namespace Characters.Enemy
{
    public class EnemyDestructionManager
    {
        private readonly List<MeshColliderAndSkinnedMeshData> _meshColliderAndSkinnedMeshDatas;
        private readonly CharacterAnimationController _animationController;
        private int _nextIndex;
        private System.Random _rng = new();
        private int _getTargetRequestCount;
        
        private bool _leglessTriggered;
        private bool _armlessTriggered;
        private bool _headlessTriggered;

        public EnemyDestructionManager(List<MeshColliderAndSkinnedMeshData> meshColliderAndSkinnedMeshDatas,
            CharacterAnimationController animationController)
        {
            _meshColliderAndSkinnedMeshDatas = meshColliderAndSkinnedMeshDatas;
            _animationController = animationController;
        }

        [Inject]
        private void Inject(CollectionArea collectionArea)
        {
            foreach (var meshColliderAndSkinnedMeshData in _meshColliderAndSkinnedMeshDatas)
            {
                meshColliderAndSkinnedMeshData.Initialize(collectionArea);
            }
        }

        public void DestroyPartIfPossible(GameObject hitObj)
        {
            if (hitObj == null) return;
            var collider = hitObj.GetComponent<MeshCollider>();
            var part = _meshColliderAndSkinnedMeshDatas.Find(i => i.CheckIfObjectIsPartOfParent(collider));
            if (part == null) return;

            part.Demolish();

            CheckLimbCount();
        }

        public void DestroyAllParts()
        {
            foreach (var meshColliderAndSkinnedMeshData in _meshColliderAndSkinnedMeshDatas)
                meshColliderAndSkinnedMeshData.Demolish();
        }

        public MeshColliderAndSkinnedMeshData GetMeshColliderToAttack()
        {
            _getTargetRequestCount++;

            var alive = _meshColliderAndSkinnedMeshDatas
                .Where(d => d != null && !d.IsDestroyed && d.ParentGameObjectOfColliders != null)
                .OrderBy(_ => _rng.Next()) // shuffle
                .ToList();

            if (alive.Count == 0)
            {
                Debug.Log("Not anything is alive");
                return null;
                
            }

            // if (_getTargetRequestCount <= 1)
            // {
            //     var nonLegAlive = alive
            //         .Where(d => d.BodyPart != BodyPart.Leg)
            //         .ToList();
            //
            //     if (nonLegAlive.Count > 0)
            //     {
            //         int i = _rng.Next(nonLegAlive.Count);
            //         return nonLegAlive[i];
            //     }
            //
            //     int fallbackIdx = _rng.Next(alive.Count);
            //     return alive[fallbackIdx];
            // }

            int idx = _rng.Next(alive.Count);
            return alive[idx];
        }


        private void CheckLimbCount()
        {
            var legsLeft = _meshColliderAndSkinnedMeshDatas
                .Count(d => d != null && !d.IsDestroyed && d.BodyPart == BodyPart.Leg);

            var armsLeft = _meshColliderAndSkinnedMeshDatas
                .Count(d => d != null && !d.IsDestroyed && d.BodyPart == BodyPart.Arm);

            var headsLeft = _meshColliderAndSkinnedMeshDatas
                .Count(d => d != null && !d.IsDestroyed && d.BodyPart == BodyPart.Head);

            if (legsLeft <= 1 && !_leglessTriggered)
            {
                _leglessTriggered = true;
                _animationController.Legless();
            }

            if (armsLeft == 0 && !_armlessTriggered)
            {
                _armlessTriggered = true;
                _animationController.Armless();
            }

            if (headsLeft == 0 && !_headlessTriggered)
            {
                _headlessTriggered = true;
                _animationController.Headless();
            }
        }
    }
}
