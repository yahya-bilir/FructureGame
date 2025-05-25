using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    [CreateAssetMenu(fileName = "RangedWeapon", menuName = "Scriptable Objects/Weapons/Ranged Weapon", order = 2)]
    public class RangedWeaponSO : UpgradableWeaponSO
    {
        [field: SerializeField] public float AttackSpeedUpgradeOnEachIncrement { get; private set; }
        [field: SerializeField] public bool ShouldDisableAfterEachShot { get; private set; }
        [field: SerializeField] public AmmoProjectile ProjectilePrefab { get; private set; }
    }
}