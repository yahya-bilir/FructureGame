using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace WeaponSystem
{
    
    [CreateAssetMenu(fileName = "UpgradeableWeapon", menuName = "Scriptable Objects/Weapons/Upgradeable Weapon", order = 0)]
    public class UpgreadableWeaponSO : WeaponSO
    {
        [field: SerializeField] public float DamageIncrementOnEachUpgrade { get; private set; }
        
        [field: SerializeField] public float AttackSpeedUpgradeOnEachIncrement { get; private set; }
    }
    
}