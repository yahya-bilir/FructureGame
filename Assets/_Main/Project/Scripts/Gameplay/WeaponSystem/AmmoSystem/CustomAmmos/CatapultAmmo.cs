using Characters;
using Characters.Enemy;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace WeaponSystem.AmmoSystem.CustomAmmos
{
    public class CatapultAmmo : RocketAmmo
    {
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override void Explode()
        {
            if (_hasExploded) return;
            _hasExploded = true;

            // 💥 VFX
            if (vfx != null)
            {
                var spawnedVfx = Instantiate(vfx, transform.position, Quaternion.identity);
                spawnedVfx.Play();
            }

            // 🎯 Parça yok etme işlemi
            var hits = Physics.OverlapSphere(transform.position, aoeRadius, LayerMask.GetMask("AI"));
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Part"))
                {
                    var parentChar = hit.GetComponentInParent<Character>();
                    if (parentChar == null || parentChar.Faction == ConnectedCombatManager.Character.Faction) continue;
                    var enemyBehaviour = parentChar as EnemyBehaviour;
                    enemyBehaviour.CharacterCombatManager.GetDamage(Damage, DamageTypes.Normal, hit.gameObject);
                    enemyBehaviour.EnemyDestructionManager.DestroyPartIfPossible(hit.gameObject);
                }
            }

            Rigidbody.linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
            _ownerWeapon.OnAmmoDestroyed(this);
        }

    }
}