using UI;
using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    [CreateAssetMenu(fileName = "AmmoProjectile", menuName = "Scriptable Objects/Weapons/Ammo Projectile", order = 3)]
    public class AmmoProjectileSO : ObjectUIIdentifierSO
    {
        [field: SerializeField] public float Speed { get; private set; }
    }
}