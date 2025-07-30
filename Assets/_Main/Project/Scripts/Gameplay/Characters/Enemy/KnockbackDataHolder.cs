using UnityEngine;

namespace Characters.Enemy
{
    [CreateAssetMenu(fileName = "KnockbackData", menuName = "Scriptable Objects/Enemy/Knockback Data", order = 4)]
    public class KnockbackDataHolder : ScriptableObject
    {
        [field: SerializeField] public float Force { get; private set; } = 25f;
        [field: SerializeField] public float Duration { get; private set; } = 0.5f;
        [field: SerializeField] public float UpwardModifier { get; private set; } = 0.25f;
    }
}