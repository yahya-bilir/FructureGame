using System;
using UnityEngine;

namespace Characters.Enemy
{
    [Serializable]
    public struct CharacterDissolveMaterial
    {
        [field: SerializeField] public Renderer MaterialHolder { get; private set; }
    }
}