using System.Collections.Generic;
using PropertySystem;
using UnityEngine;

namespace Characters.Transforming
{
    [CreateAssetMenu(fileName = "CharacterUpgradePathSO", menuName = "Scriptable Objects/Character Upgrade Path")]

    public class CharacterTransformPathSO : ScriptableObject
    {
        [field: SerializeField] public List<Character> SequentialTransformableCharacters { get; private set; }
    }
}