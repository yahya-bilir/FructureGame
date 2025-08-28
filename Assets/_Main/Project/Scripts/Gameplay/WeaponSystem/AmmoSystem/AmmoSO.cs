using UI;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    [CreateAssetMenu(fileName = "AmmoProjectile", menuName = "Scriptable Objects/Weapons/Ammo Projectile", order = 3)]
    public class AmmoSO : ObjectUIIdentifierSO
    {
        [field: SerializeField] public float Speed { get; private set; }
    }
}