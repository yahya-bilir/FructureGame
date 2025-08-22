using UnityEngine;

namespace BasicStackSystem
{
    [CreateAssetMenu(fileName = "Stack Area", menuName = "Scriptable Objects/Stack Area", order = 0)]
    public class StackAreaSO : ScriptableObject
    {
        [field: SerializeField] public Vector3 Increments { get; private set; }

        [field: SerializeField] public Vector3 ItemInitialPosition { get; private set; }

        [field: SerializeField] public int MaxItemsInColumn { get; private set; }

        [field: SerializeField] public int MaxItemsInRow { get; private set; }

        [field: SerializeField] public float TweenDuration { get; private set; }
    }
}