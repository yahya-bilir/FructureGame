using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualEffects
{
    [Serializable]
    public class TransformHolder
    {
        [field: SerializeField] public List<Transform> Transforms { get; private set; }
        public List<Vector3> InitialScales { get; set; }
        [field: SerializeField] public float WaitForSeconds { get; private set; }
    }
}