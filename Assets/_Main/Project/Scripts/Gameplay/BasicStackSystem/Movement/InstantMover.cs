using DG.Tweening;
using UnityEngine;

namespace BasicStackSystem
{
    public sealed class InstantMover : IStackMover
    {
        public void Place(Transform tr, Transform parent, Vector3 localPos)
        {
            if (tr.parent != parent) tr.SetParent(parent, false);
            tr.DOKill();
            tr.localPosition = localPos;
        }
    }
}