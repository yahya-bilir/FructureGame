using UnityEngine;
using WeaponSystem.AmmoSystem;

namespace WeaponSystem.RangedWeapons
{
    [CreateAssetMenu(fileName = "RangedWeapon", menuName = "Scriptable Objects/Weapons/Ranged Weapon", order = 2)]
    public class RangedWeaponSO : UpgreadableWeaponSO
    {
        [field: SerializeField] public bool ShouldDisableAfterEachShot { get; private set; }
        [field: SerializeField] public AmmoBase ProjectilePrefab { get; private set; }

    }
}