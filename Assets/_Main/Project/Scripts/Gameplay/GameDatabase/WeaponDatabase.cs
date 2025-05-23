using System;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

namespace DataSave
{
    [Serializable]
    public struct WeaponDatabase
    {
        [field: SerializeField] public List<WeaponStagesSO> WeaponStages { get; private set; }
        [field: SerializeField] public List<ObjectWithDamage> Weapons { get; private set; }
    }
}