using System.Collections.Generic;
using PropertySystem;
using UnityEngine;

namespace DataSave.Runtime
{
    [CreateAssetMenu(fileName = "CharacterProperties", menuName = "Scriptable Objects/Character Properties")]
    public class CharacterProperties : ScriptableObject
    {
        [field: SerializeField] public bool IsSaveable { get; private set; }
        [field: SerializeField] public string EntityId { get; private set; }
        [field: SerializeField] public List<PropertyData> PropertySaveDatas { get; private set; }
    }
}