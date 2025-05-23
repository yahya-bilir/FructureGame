using System.Collections.Generic;
using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace WeaponSystem
{
    public abstract class UpgradableWeaponSO : WeaponSO
    {
        [field: SerializeField] public float DamageIncrementOnEachUpgrade { get; private set; }
        [field: SerializeField] public float InitialAttackSpeed { get; private set; }

        [field: SerializeField] public List<WeaponStagesSO> Stages { get; private set; }
    }
    
}