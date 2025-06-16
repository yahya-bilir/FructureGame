using UI;
using UnityEngine;

namespace WeaponSystem.MeleeWeapons
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapons/Weapon", order = 0)]
    public class WeaponSO : ObjectUIIdentifierSO
    {
        [field: SerializeField] public float AttackInterval { get; private set; }

        [field: SerializeField] public float MinimumRange { get; private set; } = 0.2f;
    }
}