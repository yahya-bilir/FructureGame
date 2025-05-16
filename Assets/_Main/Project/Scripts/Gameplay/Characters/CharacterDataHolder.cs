using DataSave.Runtime;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "CharacterDataHolder", menuName = "Scriptable Objects/Character Data Holder", order = 0)]
    public class CharacterDataHolder : ScriptableObject
    {
        [field: SerializeField] public int Worth { get; private set; }
        
        [Header("Shining")]
        [field: SerializeField, ColorUsage(true, true)] public Color ShineColor { get; private set; } =  new Color(2f, 2f, 2f, 1f);
        [field: SerializeField] public float ShineDuration { get; private set; } = .3f;
        
    }
}