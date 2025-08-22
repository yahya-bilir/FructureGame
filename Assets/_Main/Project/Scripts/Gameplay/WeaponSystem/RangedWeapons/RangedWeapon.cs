using Characters;
using UnityEngine;
using WeaponSystem;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

public abstract class RangedWeapon : UpgradeableWeapon
{
    [SerializeField] protected Transform projectileCreationPoint;
    [field: SerializeField] public Animator Animator { get; private set; }

    protected RangedWeaponSO RangedWeaponSo;
    protected float _shootCooldown;

    public override void Initialize(CharacterCombatManager connectedCombatManager, float damage)
    {
        RangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
        base.Initialize(connectedCombatManager, damage);
        CurrentAttackInterval = RangedWeaponSo.AttackInterval;
    }

    public abstract void Shoot(Character character);

    public virtual void OnAmmoDestroyed(AmmoBase ammo) { }

    protected override void ApplyUpgradeEffects() { }
}