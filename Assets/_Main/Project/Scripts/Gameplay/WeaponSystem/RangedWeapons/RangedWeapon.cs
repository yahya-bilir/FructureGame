using System.Collections.Generic;
using Characters;
using UnityEngine;
using Utilities.Vibrations;
using WeaponSystem;
using WeaponSystem.RangedWeapons;

public class RangedWeapon : UpgradeableWeapon
{
    private RangedWeaponSO _rangedWeaponSo;
    private float _shootCooldown;
    private Queue<AmmoProjectile> _projectilePool;




    public override void Initialize(CharacterCombatManager connectedCombatManager)
    {
        base.Initialize(connectedCombatManager);
        _rangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
        CurrentAttackInterval = _rangedWeaponSo.AttackInterval;
        InitializePool();
    }

    private void Update()
    {
        _shootCooldown += Time.deltaTime;
        if (CurrentAttackInterval / 2 < _shootCooldown)
        {
            modelRenderer.enabled = true;
        }

        if (_shootCooldown >= CurrentAttackInterval)
        {
            var closestEnemy = ConnectedCombatManager.FindNearestEnemy();
            if (closestEnemy == null) return;
            Shoot(closestEnemy);
            _shootCooldown = 0;
        }
    }

    private void Shoot(Character character)
    {
        if (_rangedWeaponSo.ShouldDisableAfterEachShot) modelRenderer.enabled = false;
        if (_projectilePool.Count == 0) ExpandPool();
        Vibrations.Soft();
        var projectile = _projectilePool.Dequeue();
        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;
        projectile.SetNewDamage(Damage);
        projectile.gameObject.SetActive(true);
        projectile.SetOwnerAndColor(this, _currentColor);
        projectile.SendProjectileToDirection(character.transform.position - transform.position);
    }

    protected override void ApplyUpgradeEffects()
    {
        CurrentAttackInterval -= _rangedWeaponSo.AttackSpeedUpgradeOnEachIncrement;
    }

    #region Pool

    private void InitializePool()
    {
        _projectilePool = new Queue<AmmoProjectile>();
        for (int i = 0; i < 100; i++)
        {
            var projectile = Instantiate(_rangedWeaponSo.ProjectilePrefab);
            projectile.gameObject.SetActive(false);
            _projectilePool.Enqueue(projectile);
        }
    }

    public void ReturnProjectileToPool(AmmoProjectile projectile)
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
        }
    }

    #endregion
}
