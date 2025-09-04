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
        private float _speed;
        private bool _hasReturnedToPool = false;
        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();
            var so = ObjectUIIdentifierSo as AmmoSO;
            _speed = so.Speed;

        }

        public override void FireAt(Character target)
        {
            base.FireAt(target);
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = false;
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
            Rigidbody.linearVelocity = direction * _speed;
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
            else
            {
                return;
            }

            Video.Events.OnBallClashed?.Invoke(transform);
            HitVisualEffect();
            DisableAndEnqueue();

        }
    }
}
