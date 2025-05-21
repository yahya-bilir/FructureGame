using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeapon : ObjectWithDamage
    {
        private RangedWeaponSO _rangedWeaponSo;
        private float _shootCooldown;
        private float _currentAttackInterval;
        private Queue<AmmoProjectile> _projectilePool;

        private void Awake()
        {
            _rangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
            _currentAttackInterval = _rangedWeaponSo.InitialAttackSpeed;
            
            _rangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
            _currentAttackInterval = _rangedWeaponSo.InitialAttackSpeed;
    
            _projectilePool = new Queue<AmmoProjectile>();
            for (int i = 0; i < 100; i++)
            {
                var projectile = Instantiate(_rangedWeaponSo.ProjectilePrefab);
                projectile.gameObject.SetActive(false);
                _projectilePool.Enqueue(projectile);
            }
        }

        private void Update()
        {
            _shootCooldown += Time.deltaTime;
            if (_shootCooldown >= _currentAttackInterval)
            {
                var closestEnemy = ConnectedCombatManager.FindNearestEnemy();
                if(closestEnemy == null) return;
                Shoot(closestEnemy);
                _shootCooldown = 0;
            }
        }
        private void Shoot(Character character)
        {
            if (_projectilePool.Count == 0)
                ExpandPool();

            var projectile = _projectilePool.Dequeue();
            projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation;
            projectile.SetNewDamage(Damage);
            projectile.gameObject.SetActive(true);
            projectile.SetOwner(this);
            projectile.SendProjectileToDirection(character.transform.position - transform.position);
        }
        
        public void UpgradeAttackInterval()
        {
            _currentAttackInterval -= _rangedWeaponSo.AttackSpeedUpgradeOnEachIncrement;
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
    }
}