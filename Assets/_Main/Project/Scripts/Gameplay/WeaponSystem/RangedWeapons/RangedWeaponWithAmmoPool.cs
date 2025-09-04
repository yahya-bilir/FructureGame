using System.Collections.Generic;
using Characters;
using UnityEngine;
using WeaponSystem.AmmoSystem;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeaponWithAmmoPool : RangedWeapon
    {
        protected Queue<AmmoBase> _projectilePool;

        public override void Initialize(CharacterCombatManager connectedCombatManager, float damage)
        {
            base.Initialize(connectedCombatManager, damage);
            InitializePool();
        }

        public override void Shoot(Character character)
        {
            // if (_rangedWeaponSo.ShouldDisableAfterEachShot)
            //     modelRenderer.enabled = false;

            if (_projectilePool.Count == 0)
                ExpandPool();

            var ammo = _projectilePool.Dequeue();
            if (ammo == null)
            {
                Debug.LogError("Ammo is null");
                return;
            }
            ammo.transform.SetParent(null); // herhangi bir parent'tan ayrılıyor
            ammo.transform.position = ProjectileCreationPoints[0].position;
            ammo.transform.rotation = transform.rotation;
            ammo.gameObject.SetActive(true);

            ammo.SetOwnerAndColor(this, _currentColor);
            ammo.Initialize(ConnectedCombatManager, Damage);
            ammo.FireAt(character); // ✅ FireAt(Character) polimorfik çağrı
        }

        #region Pool

        private void InitializePool()
        {
            _projectilePool = new Queue<AmmoBase>();
            for (int i = 0; i < 10; i++)
            {
                var projectile = Instantiate(RangedWeaponSo.ProjectilePrefab);
                projectile.gameObject.SetActive(false);
                _projectilePool.Enqueue(projectile);
            }
        }

        public void ReturnProjectileToPool(AmmoBase projectile)
        {
            _projectilePool.Enqueue(projectile);
        }

        protected void ExpandPool(int amount = 5)
        {
            for (int i = 0; i < amount; i++)
            {
                var projectile = Instantiate(RangedWeaponSo.ProjectilePrefab);
                projectile.gameObject.SetActive(false);
                _projectilePool.Enqueue(projectile);
                Debug.LogError("Pool expanded");
            }
        }

        #endregion
    }
}