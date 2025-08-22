using BasicStackSystem;
using Characters;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;
using WeaponSystem.AmmoSystem;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeaponWithExternalAmmo : RangedWeapon
    {
        private BasicStack _connectedStack;

        [Inject]
        protected override void Inject(IEventBus eventBus)
        {
            base.Inject(eventBus);
            EventBus.Subscribe<OnStackObjectReceived>(OnStackObjectReceived);
        }

        public void SetStack(BasicStack stack) => _connectedStack = stack;

        private void OnStackObjectReceived(OnStackObjectReceived evt)
        {
            if (evt.Stack != _connectedStack) return;
        }

        public override void Shoot(Character character)
        {
            if (_connectedStack == null || !_connectedStack.IsThereAnyObject) return;

            var item = _connectedStack.EjectLastTo(transform, Vector3.zero, true);
            if (item == null) return;
            if (!item.GameObject.TryGetComponent(out AmmoBase ammo)) return;

            var t = ammo.transform;
            t.SetParent(null, true);
            t.position = projectileCreationPoint.position;
            t.rotation = transform.rotation;
            ammo.gameObject.SetActive(true);

            ammo.SetOwnerAndColor(this, _currentColor);
            ammo.Initialize(ConnectedCombatManager, Damage);
            ammo.FireAt(character);
        }

        private void OnDisable()
        {
            EventBus?.Unsubscribe<OnStackObjectReceived>(OnStackObjectReceived);
        }
    }
}