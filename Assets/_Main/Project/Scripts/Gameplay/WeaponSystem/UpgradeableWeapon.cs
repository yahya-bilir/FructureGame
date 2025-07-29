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
        
        protected IEventBus EventBus;
        protected Color _currentColor;
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            EventBus = eventBus;
            EventBus.Subscribe<OnWeaponUpgraded>(ChangeTintColor);
        }
        
        private void ChangeTintColor(OnWeaponUpgraded eventData)
        {
            if (eventData.ObjectUIIdentifierSo != ObjectUIIdentifierSo) return;
            _currentColor = eventData.Stage.OutlineColor;
            //modelRenderer.material.SetColor("_OuterOutlineColor", _currentColor);
        }
        
        public override void SetNewDamage(float damage)
        {
            base.SetNewDamage(damage);
            ApplyUpgradeEffects();
        }

        
        protected abstract void ApplyUpgradeEffects();

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnWeaponUpgraded>(ChangeTintColor);
        }
    }
}