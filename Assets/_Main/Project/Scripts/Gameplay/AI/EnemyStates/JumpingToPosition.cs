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
            _aiPath.canMove = false;

            Vector2 jumpTarget = _characterIslandController.GetNextIslandLandingPosition();
            float jumpDuration = 0.5f;
            float scaleUp = 1.2f;

            Vector3 originalScale = _modelTransform.localScale;

            // DOTween Sequence
            var jumpSequence = DG.Tweening.DOTween.Sequence();

            // 1) Zıplama pozisyonuna hareket
            jumpSequence.Append(
                _enemyBehaviour.transform
                    .DOMove(jumpTarget, jumpDuration)
                    .SetEase(Ease.OutQuad)
            );

            // 2) Scale animasyonu aynı anda (Join)
            jumpSequence.Join(
                _modelTransform
                    .DOScale(originalScale * scaleUp, jumpDuration / 2f)
                    .SetEase(Ease.OutQuad)
            );

            // 3) Scale geri dönüş (Append)
            jumpSequence.Append(
                _modelTransform
                    .DOScale(originalScale, jumpDuration / 2f)
                    .SetEase(Ease.InQuad)
            );

            // 4) Tamamlanınca StopJumping çağır
            jumpSequence.OnComplete(() =>
            {
                _characterIslandController.StopJumping();
            });

            // Opsiyonel: Jump animasyonu tetikle
            //_characterAnimationController.Jump();
        }



        public void OnExit()
        {

        }
    }
}