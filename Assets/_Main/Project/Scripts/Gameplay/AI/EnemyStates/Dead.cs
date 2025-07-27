using AI.Base.Interfaces;
using Characters;

using UnityEngine;

namespace AI.EnemyStates
{
    public class Dead : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly Collider _collider2D;
        private float _deathTimer;

        public Dead(CharacterAnimationController animationController, Collider collider2D)
        {
            _animationController = animationController;
            _collider2D = collider2D;
        }

        public void Tick()
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= 0.4f)
            {
                _collider2D.gameObject.SetActive(false);
            }
        }

        public void OnEnter()
        {
            _collider2D.enabled = false;
            //_animationController.DisableAnimator();
            _animationController.Dead();
            //_camerasManager.ShakeCamera();

            // foreach (var partPrefab in _parts)
            // {
            //     Vector3 spawnPosition = _collider2D.transform.position;
            //     GameObject partInstance = GameObject.Instantiate(partPrefab, spawnPosition, Quaternion.identity);
            //
            //     //partInstance.layer = LayerMask.NameToLayer("Parts");
            //
            //     // Yakın konuma zıplat
            //     Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(1f, 2f); // daha küçük radius
            //     Vector3 targetPosition = spawnPosition + (Vector3)randomOffset;
            //
            //     partInstance.transform.DOJump(
            //         targetPosition,
            //         jumpPower: 2f,
            //         numJumps: 1,
            //         duration: 0.5f
            //     );
            //
            //     partInstance.transform.DORotate(
            //         new Vector3(0f, 0f, Random.Range(-180f, 180f)), 
            //         0.5f, 
            //         RotateMode.FastBeyond360
            //     );
            //     
            //     var partsInstance = partInstance.GetComponent<PartsAutoDestroyer>();
            //     partsInstance.StartFade(_playerTransform);
            // }
        }


        public void OnExit()
        {
            
        }
    }
}