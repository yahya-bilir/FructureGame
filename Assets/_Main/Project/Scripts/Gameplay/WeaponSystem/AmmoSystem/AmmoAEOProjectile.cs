using System.Threading;
using Characters;
using DG.Tweening;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoAOEProjectile : AmmoBase
    {
        [SerializeField] private float aoeRadius = 3f;
        [SerializeField] private float jumpPower = 2f;
        [SerializeField] private float jumpDuration = 1f;
        [SerializeField] private ParticleSystem vfx;

        protected bool _hasExploded = false;
        protected CancellationTokenSource _cts;
        protected Rigidbody _rigidbody;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void FireAt(Character target)
        {
            FireAtPosition(target.transform.position);
        }

        protected virtual void FireAtPosition(Vector3 targetPos)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _hasExploded = false;

            transform.DOMove(targetPos, jumpDuration)
                .SetEase(Ease.Linear)
                .SetLoops(1)
                .OnComplete(() => Explode());
        }

        protected void Explode()
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

            _rigidbody.linearVelocity = Vector3.zero;

            gameObject.SetActive(false);
            _ownerWeapon.OnAmmoDestroyed(this);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering) { }
        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}
