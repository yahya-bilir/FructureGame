using System;
using System.Threading;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Characters.Enemy;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoProjectile : AmmoBase
    {
        protected Rigidbody _rigidbody;
        private float _speed;
        private bool _hasReturnedToPool = false;
        private CancellationTokenSource _cts;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            var so = ObjectUIIdentifierSo as AmmoSO;
            _speed = so.Speed;
            _rigidbody.useGravity = false;
        }

        public override void FireAt(Character target)
        {
            _hasReturnedToPool = false;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            Vector3 aimPoint;

            if (target is EnemyBehaviour eb)
            {
                var data = eb.EnemyDestructionManager.GetMeshColliderToAttack();
                if (data != null && data.ParentGameObjectOfColliders != null)
                {
                    aimPoint = data.ParentGameObjectOfColliders.transform.position;
                }
                else
                {
                    Debug.Log("Data is null or not any parent is placed");

                    aimPoint = target.transform.position + (Vector3.up);
                }
            }
            else
            {
                aimPoint = target.transform.position + (Vector3.up);
            }
            
            Vector3 direction = (aimPoint - transform.position).normalized;
            _rigidbody.linearVelocity = direction * _speed;
            transform.rotation = Quaternion.LookRotation(direction);

            AutoDisableAfterTime(_cts.Token).Forget();
        }

        private async UniTaskVoid AutoDisableAfterTime(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(1500, cancellationToken: token);
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
            _ownerWeapon.OnAmmoDestroyed(this);
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            //
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering)
        {
            if(!isEntering) return;
            Debug.Log(other.name);
            if (other.CompareTag("Part"))
            {
                var parentChar = other.GetComponentInParent<Character>();
                if(parentChar.Faction == ConnectedCombatManager.Character.Faction) return;
                parentChar.CharacterCombatManager.GetDamage(Damage, DamageTypes.Normal, other.gameObject);
            }
            else if (other.CompareTag("Enemy"))
            {
                var comp = other.GetComponent<Character>();
                comp.CharacterCombatManager.GetDamage(Damage);
            }
            
            DisableAndEnqueue();

        }
    }
}
