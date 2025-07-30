using UnityEngine;

namespace Characters.Enemy
{
    [CreateAssetMenu(fileName = "RagdollData", menuName = "Scriptable Objects/Enemy/Ragdoll Data", order = 3)]
    public class RagdollDataHolder : ScriptableObject
    {
        [field: SerializeField] public float Force { get; private set; } = 50f;
        [field: SerializeField] public float Radius { get; private set; } = 2f;
        [field: SerializeField] public float UpwardsModifier { get; private set; } = 0.5f;
    }
}