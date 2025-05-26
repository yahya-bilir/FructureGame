using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace WeaponSystem
{
    public abstract class UpgradableWeaponSO : WeaponSO
    {
        [field: SerializeField] public float DamageIncrementOnEachUpgrade { get; private set; }
    }
    
}