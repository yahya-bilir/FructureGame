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

        private bool _hasExploded = false;
        private CancellationTokenSource _cts;

        public override void FireAt(Character target)
        {
            FireAtPosition(target.transform.position);
        }

        public void FireAtPosition(Vector3 targetPos)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _hasExploded = false;

            transform.DOMove(targetPos, jumpDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(1)
                .OnComplete(() => Explode());
        }

        private void Explode()
        {
            if (_hasExploded) return;
            _hasExploded = true;

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

            gameObject.SetActive(false);
            _ownerWeapon.ReturnProjectileToPool(this);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering) { }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}