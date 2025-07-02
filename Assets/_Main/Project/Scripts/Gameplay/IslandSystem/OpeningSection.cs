using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace IslandSystem
{
    [Serializable]
    public struct OpeningSection
    {
        [field: SerializeField] public List<Character> Enemies { get; private set; }
    }
}