using BasicStackSystem;
using Characters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using UnityEngine;
using VContainer;
using WeaponSystem.AmmoSystem;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeaponWithExternalAmmo : RangedWeapon
    {
        public bool IsLoaded { get; private set; }
        public AmmoBase LoadedAmmo { get; private set; }

        [field: SerializeField] public Transform CarrierDropPoint { get; private set; }
        
        [Inject]
        protected override void Inject(IEventBus eventBus)
        {
            base.Inject(eventBus);
        }

        public override void Shoot(Character character)
        {
            if (LoadedAmmo == null) return;
            var t = LoadedAmmo.transform;
            t.SetParent(null, true);
            t.position = projectileCreationPoint.position;
            t.rotation = transform.rotation;
            LoadedAmmo.gameObject.SetActive(true);

            LoadedAmmo.SetOwnerAndColor(this, _currentColor);
            LoadedAmmo.Initialize(ConnectedCombatManager, Damage);
            LoadedAmmo.FireAt(character);
            UnloadWeapon();
        }

        public async UniTask LoadWeapon(AmmoBase ammo)
        {
            var trf = ammo.transform;
            trf.SetParent(projectileCreationPoint);
            IsLoaded = true;
            await trf.DOLocalJump(Vector3.zero, 1, 1, 0.5f).ToUniTask();
            LoadedAmmo = ammo;
        }
        
        private void UnloadWeapon()
        {
            IsLoaded = false;
            LoadedAmmo = null;
        }

        private void Update()
        {
            //Debug.Log($"IsLoaded: {IsLoaded} |  LoadedAmmo: {LoadedAmmo}");
        }
    }
}