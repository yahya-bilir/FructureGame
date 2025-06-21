using System;
using System.Collections.Generic;
using Characters.Transforming;
using UnityEngine;

namespace Database
{
    [Serializable]
    public struct CharacterTransformPathDatabase
    {
        [field: SerializeField] public List<CharacterTransformPathSO> CharacterUpgradePaths { get; private set; }
    }
}