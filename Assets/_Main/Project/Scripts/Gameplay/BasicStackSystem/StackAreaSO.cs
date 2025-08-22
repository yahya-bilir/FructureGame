using UnityEngine;

namespace BasicStackSystem
{
    [CreateAssetMenu(fileName = "Stack Area", menuName = "Stack Area", order = 0)]
    public class StackAreaSO : ScriptableObject
    {
        [field: SerializeField] public Vector3 Increments { get; private set; }

        [field: SerializeField] public Vector3 ItemInitialPosition { get; private set; }

        [field: SerializeField] public int MaxItemsInColumn { get; private set; }

        [field: SerializeField] public int MaxItemsInRow { get; private set; }

        [field: SerializeField] public float JumpSpeedInSeconds { get; private set; }
    }
}