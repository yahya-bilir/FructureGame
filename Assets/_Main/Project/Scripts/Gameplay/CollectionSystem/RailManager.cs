using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using WeaponSystem.AmmoSystem;

namespace CollectionSystem
{
    public class RailManager : MonoBehaviour
    {
        
    }

    [Serializable]
    public class BallWaitingArea
    {
        [field: SerializeField] public SplineComputer Spline { get; private set; }
        [field: SerializeField] public List<GameObject> ObjectsToActivate { get; private set; }
        private readonly List<AmmoBase> _collectedAmmos = new(); 
        private RailManager _railManager;
        
        public void Initialize(RailManager railManager) => _railManager = railManager;
    }
}