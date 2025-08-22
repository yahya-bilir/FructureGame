using Characters;
using DG.Tweening;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public abstract class AmmoHomingBase : AmmoBase
    {
        protected float _speed;
        protected Tween _moveTween;
        protected Character _targetCharacter;

        private void Awake()
        {
            var so = ObjectUIIdentifierSo as AmmoSO;
            _speed = so.Speed;
        }

        public override void FireAt(Character target)
        {
            if (target == null) return;

            _targetCharacter = target;

            transform.SetParent(target.transform, worldPositionStays: true);
            PlayTween(target);
        }

        protected abstract void PlayTween(Character target);

        private void OnDisable()
        {
            _moveTween?.Kill();
        }

        protected void OnTweenComplete()
        {
            if (_targetCharacter != null)
                _targetCharacter.CharacterCombatManager.GetDamage(Damage);

            gameObject.SetActive(false);
            _ownerWeapon.OnAmmoDestroyed(this);
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            //if (!isEntering) return;
            //Debug.Log($"Homing ammo hit something: {other.name}");
        }
    }
}