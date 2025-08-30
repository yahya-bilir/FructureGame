using System.Threading;
using Characters;
using DG.Tweening;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public abstract class AmmoAOEProjectile : AmmoBase
    {
        [SerializeField] protected float aoeRadius = 3f;
        [SerializeField] protected ParticleSystem vfx;

        protected bool _hasExploded = false;
        protected CancellationTokenSource _cts;

        public override void FireAt(Character target)
        {
            FireAtPosition(target.transform.position);
        }

        protected abstract void FireAtPosition(Vector3 targetPos);

        protected virtual void Explode()
        {
            if (_hasExploded) return;
            _hasExploded = true;

            // 💥 VFX oluştur
            if (vfx != null)
            {
                vfx.transform.SetParent(null);
                vfx.transform.position = transform.position;
                vfx.Play();
            }

            // 📦 Hasar verilecek hedefleri bul
            var hits = Physics.OverlapSphere(transform.position, aoeRadius, LayerMask.GetMask("AI"));
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Character enemy) &&
                    enemy.Faction != ConnectedCombatManager.Character.Faction &&
                    !enemy.IsCharacterDead)
                {
                    enemy.CharacterCombatManager.GetDamage(Damage);
                }
            }

            Rigidbody.linearVelocity = Vector3.zero;

            gameObject.SetActive(false);
            _ownerWeapon.OnAmmoDestroyed(this);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering) { }
        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}
