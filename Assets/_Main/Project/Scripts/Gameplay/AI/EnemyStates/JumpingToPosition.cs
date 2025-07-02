using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
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

        public JumpingToPosition(CharacterIslandController characterIslandController, AIPath aiPath,
            Transform modelTransform, EnemyBehaviour enemyBehaviour,
            CharacterAnimationController characterAnimationController)
        {
            _characterIslandController = characterIslandController;
            _aiPath = aiPath;
            _modelTransform = modelTransform;
            _enemyBehaviour = enemyBehaviour;
            _characterAnimationController = characterAnimationController;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            _characterAnimationController.DisableAnimator();

            _aiPath.canMove = false;

            Vector2 jumpTarget = _characterIslandController.GetNextIslandLandingPosition();
            float jumpDuration = 0.5f;
            float hangTime = 0.1f;
            float scaleUp = 2.25f;

            Vector3 originalScale = _modelTransform.localScale;

            Vector2 peakPosition = new Vector2(
                jumpTarget.x,
                jumpTarget.y - 0.5f // Sabit yükselme miktarı
            );

            var jumpSequence = DOTween.Sequence();

            jumpSequence.Append(
                _enemyBehaviour.transform
                    .DOMove(peakPosition, jumpDuration / 2f)
                    .SetEase(Ease.OutQuad)
            );

            jumpSequence.Join(
                _modelTransform.DOScale(originalScale * scaleUp, jumpDuration / 2f).SetEase(Ease.OutQuad)
            );
            
            jumpSequence.AppendInterval(hangTime);

            jumpSequence.Append(
                _enemyBehaviour.transform
                    .DOMove(new Vector3(jumpTarget.x, jumpTarget.y, _enemyBehaviour.transform.position.z), jumpDuration / 2f)
                    .SetEase(Ease.InQuad)
            );

            jumpSequence.Join(_modelTransform.DOScale(originalScale, jumpDuration / 2f).SetEase(Ease.InQuad));

            jumpSequence.OnComplete(() =>
            {
                _characterAnimationController.EnableAnimator();
                _characterIslandController.StopJumping();
            });
        }

        

        public void OnExit()
        {
        }
    }
}