using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeapon : ObjectWithDamage
    {
        private RangedWeaponSO _rangedWeaponSo;
        private float _shootCooldown;
        public float CurrentAttackInterval { get; private set; }
        private Queue<AmmoProjectile> _projectilePool;

        public override void Initialize(CharacterCombatManager connectedCombatManager)
        {
            base.Initialize(connectedCombatManager);
            _rangedWeaponSo = ObjectUIIdentifierSo as RangedWeaponSO;
            CurrentAttackInterval = _rangedWeaponSo.InitialAttackSpeed;
            InitializePool();
        }

        private void Update()
        {
            _shootCooldown += Time.deltaTime;
            if (_shootCooldown >= CurrentAttackInterval)
            {
                var closestEnemy = ConnectedCombatManager.FindNearestEnemy();
                if(closestEnemy == null) return;
                Shoot(closestEnemy);
                _shootCooldown = 0;
            }
        }
        private void Shoot(Character character)
        {
            if (_projectilePool.Count == 0) ExpandPool();

            var projectile = _projectilePool.Dequeue();
            projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation;
            projectile.SetNewDamage(Damage);
            projectile.gameObject.SetActive(true);
            projectile.SetOwner(this);
            projectile.SendProjectileToDirection(character.transform.position - transform.position);
        }

        public override void SetNewDamage(float damage)
        {
            base.SetNewDamage(damage);
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
}