using Characters;
using UnityEngine;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

public class AOEWeapon : RangedWeapon
{
    public override void Shoot(Character character)
    {
        if (_projectilePool.Count == 0)
            ExpandPool();

        var ammo = _projectilePool.Dequeue() as AmmoAOEProjectile;
        if (ammo == null)
        {
            Debug.LogError("Ammo is null or wrong type");
            return;
        }

        ammo.transform.SetParent(null);
        ammo.transform.position = projectileCreationPoint.position;
        ammo.transform.rotation = Quaternion.identity;
        ammo.gameObject.SetActive(true);

        ammo.SetOwnerAndColor(this, _currentColor);
        ammo.Initialize(ConnectedCombatManager, Damage);
        ammo.FireAt(character);
    }
}