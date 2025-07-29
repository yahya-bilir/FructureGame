using System;
using System.Threading;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoProjectile : AmmoBase
    {
        private Rigidbody _rigidbody;
        private float _speed;
        private bool _hasReturnedToPool = false;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            var so = ObjectUIIdentifierSo as AmmoProjectileSO;
            _speed = so.Speed;
            _rigidbody.useGravity = false;
        }

        public override void FireAt(Character target)
        {
            _hasReturnedToPool = false;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            Vector3 direction = (target.transform.position - transform.position).normalized;
            _rigidbody.linearVelocity = direction * _speed;
            transform.rotation = Quaternion.LookRotation(direction);

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
            //
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering)
        {
            if (!isEntering || !other.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Character character)) return;
            if (character == ConnectedCombatManager.Character) return;

            character.CharacterCombatManager.GetDamage(Damage);
            DisableAndEnqueue();
        }
    }
}
