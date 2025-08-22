using UnityEngine;

namespace BasicStackSystem
{
    public interface IStackMover
    {
        void Place(Transform tr, Transform parent, Vector3 localPos);
    }
}