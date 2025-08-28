using System;
using BasicStackSystem;
using Characters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        public bool IsLoaded { get; private set; }
        private AmmoBase _loadedAmmo;    
        [Inject]
        protected override void Inject(IEventBus eventBus)
        {
            base.Inject(eventBus);
        }

        public void SetStack(BasicStack stack) => _connectedStack = stack;
        

        public override void Shoot(Character character)
        {
            if (_loadedAmmo == null) return;
            var t = _loadedAmmo.transform;
            t.SetParent(null, true);
            t.position = projectileCreationPoint.position;
            t.rotation = transform.rotation;
            _loadedAmmo.gameObject.SetActive(true);

            _loadedAmmo.SetOwnerAndColor(this, _currentColor);
            _loadedAmmo.Initialize(ConnectedCombatManager, Damage);
            _loadedAmmo.FireAt(character);
            UnloadWeapon();
        }

        public async UniTask LoadWeapon(AmmoBase ammo)
        {
            var trf = ammo.transform;
            trf.SetParent(projectileCreationPoint);
            await trf.DOLocalJump(Vector3.zero, 1, 1, 0.5f).ToUniTask();
            _loadedAmmo = ammo;
            IsLoaded = true;
        }
        
        private void UnloadWeapon()
        {
            IsLoaded = false;
            _loadedAmmo = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var ammo = _connectedStack.EjectLastTo(_connectedStack.transform, Vector3.zero, true);
                LoadWeapon(ammo as AmmoBase).Forget();
            }
        }
    }
}