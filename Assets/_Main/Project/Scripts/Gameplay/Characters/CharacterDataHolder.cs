using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

namespace Characters
{
    [CreateAssetMenu(fileName = "CharacterDataHolder", menuName = "Scriptable Objects/Character Data Holder", order = 0)]
    public class CharacterDataHolder : ScriptableObject
    {
        [field: SerializeField] public List<GameObject> OnDeathParts { get; private set; }
        
        [field: SerializeField] public int Worth { get; private set; }
        
        [Header("Shining")]
        [field: SerializeField, ColorUsage(true, true)] public Color ShineColor { get; private set; }
        [field: SerializeField] public float ShineDuration { get; private set; } = .3f;
        
        [field: SerializeField] public Color OnAttackedVFXColor { get; private set; }
        [field: SerializeField] public float OnAttackedSpeedDivider { get; private set; } = 1f;
        [field: SerializeField] public float OnAttackedSpeedRecoverTime { get; set; } = 3f;


        [Header("Attacking")]
        [field: SerializeField]
        public float AttackingInterval { get; private set; }

        [field: SerializeField] public ObjectWithDamage Weapon { get; private set; }
    }
}