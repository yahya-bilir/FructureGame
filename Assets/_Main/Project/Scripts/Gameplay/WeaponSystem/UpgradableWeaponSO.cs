using UI;
using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace WeaponSystem
{
    public abstract class UpgradableWeaponSO : WeaponSO
    {
        [field: SerializeField] public float DamageIncrementOnEachUpgrade { get; private set; }
        [field: SerializeField] public float InitialAttackSpeed { get; private set; }

    }
}