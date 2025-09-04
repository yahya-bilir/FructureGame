using Characters;
using UnityEngine;
using WeaponSystem;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;
using System.Linq;

public abstract class RangedWeapon : UpgradeableWeapon
{
    [field: SerializeField] public Transform[] ProjectileCreationPoints { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }

    public RangedWeaponSO RangedWeaponSo { get; protected set; }
    protected float _shootCooldown;

    protected Transform PrimaryCreationPoint =>
        ProjectileCreationPoints != null && ProjectileCreationPoints.Length > 0
            ? ProjectileCreationPoints[0]
            : null;

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