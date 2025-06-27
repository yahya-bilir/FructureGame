using System.Collections.Generic;
using Characters;
using UnityEngine;
using WeaponSystem;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

public class RangedWeapon : UpgradeableWeapon
{
    private RangedWeaponSO _rangedWeaponSo;
    private float _shootCooldown;
    private Queue<AmmoBase> _projectilePool;

    public override void Initialize(CharacterCombatManager connectedCombatManager, float damage)
    {
        _rangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
        base.Initialize(connectedCombatManager, damage);
        CurrentAttackInterval = _rangedWeaponSo.AttackInterval;
        InitializePool();
    }

    private void Update()
    {
        _shootCooldown += Time.deltaTime;

        if (_shootCooldown >= CurrentAttackInterval)
        {
            var closestEnemy = ConnectedCombatManager.FindNearestEnemy();
            if (closestEnemy == null) return;
            Shoot(closestEnemy);
            _shootCooldown = 0;
        }
        else if (_shootCooldown >= CurrentAttackInterval / 2)
        {
            modelRenderer.enabled = true;
        }
    }

    private void Shoot(Character character)
    {
        if (_rangedWeaponSo.ShouldDisableAfterEachShot)
            modelRenderer.enabled = false;

        if (_projectilePool.Count == 0)
            ExpandPool();

        var ammo = _projectilePool.Dequeue();
        if (ammo == null)
        {
            Debug.LogError("Ammo is null");
            return;
        }
        ammo.transform.SetParent(null); // herhangi bir parent'tan ayrılıyor
        ammo.transform.position = transform.position;
        ammo.transform.rotation = transform.rotation;
        ammo.gameObject.SetActive(true);

        ammo.SetOwnerAndColor(this, _currentColor);
        ammo.Initialize(ConnectedCombatManager, Damage);
        ammo.FireAt(character); // ✅ FireAt(Character) polimorfik çağrı
    }

    protected override void ApplyUpgradeEffects()
    {
        // Örn: CurrentAttackInterval -= _rangedWeaponSo.AttackSpeedUpgradeOnEachIncrement;
    }

    #region Pool

    private void InitializePool()
    {
        _projectilePool = new Queue<AmmoBase>();
        for (int i = 0; i < 10; i++)
        {
            var projectile = Instantiate(_rangedWeaponSo.ProjectilePrefab);
            projectile.gameObject.SetActive(false);
            _projectilePool.Enqueue(projectile);
        }
    }

    public void ReturnProjectileToPool(AmmoBase projectile)
    {
        _projectilePool.Enqueue(projectile);
    }

    private void ExpandPool(int amount = 5)
    {
        for (int i = 0; i < amount; i++)
        {
            var projectile = Instantiate(_rangedWeaponSo.ProjectilePrefab);
            projectile.gameObject.SetActive(false);
            _projectilePool.Enqueue(projectile);
            Debug.LogError("Pool expanded");
        }
    }

    #endregion
}
