using System;
using System.Threading;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoProjectile : AmmoBase
    {
        private Rigidbody2D _rigidbody;
        private float _speed;
        private bool _hasReturnedToPool = false;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            var so = ObjectUIIdentifierSo as AmmoProjectileSO;
            _speed = so.Speed;
        }

        public override void FireAt(Character target)
        {
            _hasReturnedToPool = false;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            Vector2 direction = (target.transform.position - transform.position).normalized;
            _rigidbody.gravityScale = 0;
            _rigidbody.linearVelocity = direction * _speed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            AutoDisableAfterTime(_cts.Token).Forget();
        }

        private async UniTaskVoid AutoDisableAfterTime(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(5000, cancellationToken: token);
                DisableAndEnqueue();
            }
            catch (OperationCanceledException) { }
        }

        private void DisableAndEnqueue()
        {
            if (_hasReturnedToPool) return;
            _hasReturnedToPool = true;
            _cts?.Cancel();
            gameObject.SetActive(false);
            _ownerWeapon.ReturnProjectileToPool(this);
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            if (!isEntering || !other.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Character character)) return;
            if (character == ConnectedCombatManager.Character) return;

            character.CharacterCombatManager.GetDamage(Damage);
            DisableAndEnqueue();
        }
    }
}
