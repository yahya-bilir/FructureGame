using System.Collections.Generic;
using BasicStackSystem;
using Characters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using System.Linq;
using UnityEngine;
using VContainer;
using WeaponSystem.AmmoSystem;
using WeaponSystem.AmmoSystem.Logic;

namespace WeaponSystem.RangedWeapons
{
    public class RangedWeaponWithExternalAmmo : RangedWeapon
    {
        [field: SerializeField] public AmmoLogicType AmmoLogicType { get; private set; }
        [SerializeField] private bool disableAmmoAfterPlacing;
        [SerializeField] private Transform[] projectileCreationPoints;

        private AmmoBase[] _loadedAmmos;
        public bool IsLoaded => _loadedAmmos.All(a => a != null);
        public IReadOnlyList<AmmoBase> LoadedAmmos => _loadedAmmos;

        public Transform CarrierDropPoint { get; private set; }

        [Inject]
        protected override void Inject(IEventBus eventBus)
        {
            base.Inject(eventBus);
        }

        private void Awake()
        {
            _loadedAmmos = new AmmoBase[projectileCreationPoints.Length];
        }

        public override void Shoot(Character character)
        {
            for (int i = 0; i < _loadedAmmos.Length; i++)
            {
                var ammo = _loadedAmmos[i];
                if (ammo == null) continue;

                var point = projectileCreationPoints[i];
                ammo.transform.SetParent(null, true);
                ammo.transform.position = point.position;
                ammo.transform.rotation = transform.rotation;

                ammo.gameObject.SetActive(true);
                ammo.SetOwnerAndColor(this, _currentColor);
                ammo.Initialize(ConnectedCombatManager, Damage);
                ammo.FireAt(character);

                _loadedAmmos[i] = null;
            }
        }

        public async UniTask LoadWeapon(GameObject visualObject, AmmoBase ammoPrefab, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= projectileCreationPoints.Length) return;
            if (_loadedAmmos[slotIndex] != null) return;

            var targetPoint = projectileCreationPoints[slotIndex];
            var trf = visualObject.transform;
            trf.SetParent(targetPoint);

            await trf.DOLocalJump(Vector3.zero, 1, 1, 0.5f).ToUniTask();
            Destroy(visualObject);

            var spawnedAmmo = Instantiate(ammoPrefab, targetPoint.position, targetPoint.rotation, targetPoint);
            spawnedAmmo.SetOwnerAndColor(this, _currentColor);
            spawnedAmmo.Initialize(ConnectedCombatManager, Damage);
            if (disableAmmoAfterPlacing) spawnedAmmo.gameObject.SetActive(false);

            _loadedAmmos[slotIndex] = spawnedAmmo;
            Video.Events.OnBallSpawned?.Invoke(spawnedAmmo.transform);
        }

        public void UnloadWeapon(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _loadedAmmos.Length) return;
            _loadedAmmos[slotIndex] = null;
        }

        public int? GetFirstEmptySlotIndex()
        {
            for (int i = 0; i < _loadedAmmos.Length; i++)
            {
                if (_loadedAmmos[i] == null) return i;
            }
            return null;
        }

        public void SetLoadingPos(Transform target) => CarrierDropPoint = target;
    }
}
