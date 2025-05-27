using System.Collections.Generic;
using AI.Base.Interfaces;
using Characters;
using CommonComponents;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Dead : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly Collider2D _collider2D;
        private readonly AIPath _aiPath;
        private readonly CamerasManager _camerasManager;
        private readonly List<GameObject> _parts;
        private float _deathTimer;

        public Dead(CharacterAnimationController animationController, Collider2D collider2D, AIPath aiPath,
            CamerasManager camerasManager, List<GameObject> parts)
        {
            _animationController = animationController;
            _collider2D = collider2D;
            _aiPath = aiPath;
            _camerasManager = camerasManager;
            _parts = parts;
        }

        public void Tick()
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= 1f)
            {
                GameObject.Destroy(_collider2D.gameObject);
            }
        }

        public void OnEnter()
        {
            _collider2D.enabled = false;
            _animationController.DisableAnimator();
            _aiPath.radius = 0f;
            _camerasManager.ShakeCamera();

            foreach (var partPrefab in _parts)
            {
                Vector3 spawnPosition = _collider2D.transform.position;
                GameObject partInstance = GameObject.Instantiate(partPrefab, spawnPosition, Quaternion.identity);

                //partInstance.layer = LayerMask.NameToLayer("Parts");

                // Yakın konuma zıplat
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(0.5f, 1f); // daha küçük radius
                Vector3 targetPosition = spawnPosition + (Vector3)randomOffset;

                partInstance.transform.DOJump(
                    targetPosition,
                    jumpPower: 1f,
                    numJumps: 1,
                    duration: 0.5f
                );

                partInstance.transform.DORotate(
                    new Vector3(0f, 0f, Random.Range(-180f, 180f)), 
                    0.5f, 
                    RotateMode.FastBeyond360
                );

                // Otomatik dissolve sistemi ekleniyor
                if (partInstance.GetComponent<PartsAutoDestroyer>() == null)
                {
                    partInstance.AddComponent<PartsAutoDestroyer>();
                }
            }
        }


        public void OnExit()
        {
            
        }
    }
}