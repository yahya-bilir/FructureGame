using System;
using Characters;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace WeaponSystem
{
    public abstract class UpgradeableWeapon : ObjectWithDamage
    {
        public float CurrentAttackInterval { get; protected set; }
        
        private IEventBus _eventBus;
        protected Color _currentColor;
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnWeaponUpgraded>(ChangeTintColor);
        }
        
        private void ChangeTintColor(OnWeaponUpgraded eventData)
        {
            if (eventData.ObjectUIIdentifierSo != ObjectUIIdentifierSo) return;
            _currentColor = eventData.Stage.OutlineColor;
            modelRenderer.material.SetColor("_OuterOutlineColor", _currentColor);
        }
        
        public override void SetNewDamage(float damage)
        {
            base.SetNewDamage(damage);
            ApplyUpgradeEffects();
        }

        
        protected abstract void ApplyUpgradeEffects();

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnWeaponUpgraded>(ChangeTintColor);
        }
    }
}