using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class JumpingToPosition : IState
    {
        private readonly CharacterIslandController _characterIslandController;
        private readonly AIPath _aiPath;
        private readonly Transform _modelTransform;
        private readonly EnemyBehaviour _enemyBehaviour;
        private readonly CharacterAnimationController _characterAnimationController;
        private readonly Collider2D _collider;
        private Vector2 _jumpTarget;
        private float _timer;
        public JumpingToPosition(CharacterIslandController characterIslandController, AIPath aiPath,
            Transform modelTransform, EnemyBehaviour enemyBehaviour,
            CharacterAnimationController characterAnimationController, Collider2D collider)
        {
            _characterIslandController = characterIslandController;
            _aiPath = aiPath;
            _modelTransform = modelTransform;
            _enemyBehaviour = enemyBehaviour;
            _characterAnimationController = characterAnimationController;
            _collider = collider;
        }

        public void Tick()
        {
            if (_timer <= 0.1f)
            {
                _timer += Time.deltaTime;
                return;
            }
            
            if(_aiPath.reachedDestination) _characterIslandController.StopJumping();
        }

        public void OnEnter()
        {
            Debug.Log("jumping", _enemyBehaviour);
            _collider.isTrigger = true;
            _characterIslandController.StartJumpingActions();
            _characterAnimationController.Run();
            _aiPath.canMove = true;

            Vector2 startPos = _enemyBehaviour.transform.position;
            _jumpTarget = _characterIslandController.GetNextIslandLandingPosition();
            _jumpTarget = (Vector2) _modelTransform.position + new Vector2(0, 5f);
            float jumpDuration = 0.35f;
            float hangTime = 0.1f;
            float scaleUp = 2.25f;

            Vector3 originalScale = _modelTransform.localScale;
            _aiPath.destination = _jumpTarget;
            // Sola doğru basit bir arch: X lineer, Y parabolik
            //JumpAsync(startPos, jumpTarget, jumpDuration, hangTime, originalScale, scaleUp).Forget();
        }

        private async UniTask JumpAsync(Vector2 startPos, Vector2 targetPos, float duration, float hangTime, Vector3 originalScale, float scaleUp)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // X lineer ilerleme
                float x = Mathf.Lerp(startPos.x, targetPos.x, t);

                // Y arch: basit parabol (4 * h * t * (1 - t))
                float archHeight = 0.5f;
                float y = Mathf.Lerp(startPos.y, targetPos.y, t) + archHeight * 4 * t * (1 - t);

                _enemyBehaviour.transform.position = new Vector3(x, y, _enemyBehaviour.transform.position.z);

                // Scale: Yine t’ye bağlı büyüme ve küçülme
                float scaleT = t < 0.5f ? (t / 0.5f) : ((1 - t) / 0.5f);
                _modelTransform.localScale = Vector3.Lerp(originalScale, originalScale * scaleUp, scaleT);

                await UniTask.Yield();
            }

            // Hang time (havada asılı kalma)
            await UniTask.Delay(System.TimeSpan.FromSeconds(hangTime));

            // Final pozisyonu düzelt
            _enemyBehaviour.transform.position = new Vector3(targetPos.x, targetPos.y, _enemyBehaviour.transform.position.z);
            _modelTransform.localScale = originalScale;

            _characterAnimationController.EnableAnimator();
            _characterIslandController.StopJumping();
        }
        

        public void OnExit()
        {
            _collider.isTrigger = false;
            Debug.Log("jumping disabled", _enemyBehaviour);
            _characterIslandController.SetCanJumpDisabled();
        }
    }
}