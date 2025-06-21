using System.Collections.Generic;
using UnityEngine;

namespace PropertySystem
{
    [CreateAssetMenu(fileName = "CharacterProperties", menuName = "Scriptable Objects/Character Properties")]
    public class CharacterPropertiesSO : ScriptableObject
    {
        [field: SerializeField] public bool IsSaveable { get; private set; } = true;
        [field: SerializeField] public string EntityId { get; private set; }
        [field: SerializeField] public List<PropertyData> PropertySaveDatas { get; private set; }
    }
}