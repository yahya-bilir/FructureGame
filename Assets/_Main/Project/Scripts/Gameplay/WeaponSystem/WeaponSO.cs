using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapons/Weapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public Sprite WeaponSprite { get; private set; }
        [field: SerializeField] public int DamageIncrementOnEachUpgrade { get; private set; }
        [field: SerializeField] public float AttackInterval { get; private set; }
    }
}