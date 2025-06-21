using System;
using UnityEngine;

namespace Database
{
    [Serializable]
    public struct EnhanceButtonDatabase
    {
        [field: SerializeField] public int InitialEnhancePrice { get; private set; }
        [field: SerializeField] public int IncrementOnEachUpgrade { get; private set; }
    }
}