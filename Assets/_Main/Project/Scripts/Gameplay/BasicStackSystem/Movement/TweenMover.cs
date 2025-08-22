using DG.Tweening;
using UnityEngine;

namespace BasicStackSystem
{
    public sealed class TweenMover : IStackMover
    {
        private readonly MoveStyle _style;
        private readonly float _duration;
        public TweenMover(MoveStyle style, float duration)
        {
            _style = style;
            _duration = Mathf.Max(0.01f, duration);
        }

        public void Place(Transform tr, Transform parent, Vector3 localPos)
        {
            if (tr.parent != parent) tr.SetParent(parent);
            tr.DOKill();

            switch (_style)
            {
                case MoveStyle.Move:
                    tr.DOLocalMove(localPos, _duration).SetEase(Ease.OutQuad);
                    break;
                case MoveStyle.Jump:
                    tr.DOLocalJump(localPos, 1f, 1, _duration).SetEase(Ease.OutQuad);
                    break;
                default:
                    tr.localPosition = localPos;
                    break;
            }
        }
    }
}