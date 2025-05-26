using _Main.Project.Scripts.Utils;
using Characters;
using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    public class AmmoProjectile : TriggerWeapon
    {
        private Rigidbody2D _rigidbody;
        private float _speed;

        private RangedWeapon _ownerWeapon;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            var so = ObjectUIIdentifierSo as AmmoProjectileSO;
            _speed = so.Speed;
        }

        public void SetOwnerAndColor(RangedWeapon owner, Color color)
        {
            _ownerWeapon = owner;
            modelRenderer.material.SetColor("_OuterOutlineColor", color);
            var trailRenderer = GetComponent<TrailRenderer>();
            color.a /= 2;
            trailRenderer.startColor = color;
            trailRenderer.endColor = color;
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            if (!isEntering || !other.CompareTag(Tags.Enemy)) return;

            var enemy = other.GetComponent<Character>();
            enemy.CharacterCombatManager.GetDamage(Damage);
            gameObject.SetActive(false);
            _ownerWeapon.ReturnProjectileToPool(this);
        }

        public void SendProjectileToDirection(Vector2 direction)
        {
            _rigidbody.gravityScale = 0;
            direction.Normalize(); 
            _rigidbody.linearVelocity = direction * _speed;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        
    }
}