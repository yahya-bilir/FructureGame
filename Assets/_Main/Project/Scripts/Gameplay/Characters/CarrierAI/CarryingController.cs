using BasicStackSystem;
using Characters.StationaryGunHolders;
using CollectionSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

namespace Characters.CarrierAI
{
    public class CarryingController
    {
        private static readonly int CarryHash = Animator.StringToHash("Carry");
        private readonly Transform _carryingPosition;
        private readonly PhysicsStack _stack;
        private readonly Animator _animator;
        private readonly RangedWeaponWithExternalAmmo _weapon;
        private readonly AmmoCreator _ammoCreator;
        private IStackable _carriedAmmo;
        public bool IsCarrying => _carriedAmmo != null;

        public CarryingController(Transform carryingPosition, PhysicsStack stack, Animator animator,
            RangedWeaponWithExternalAmmo weapon, AmmoCreator ammoCreator)
        {
            _carryingPosition = carryingPosition;
            _stack = stack;
            _animator = animator;
            _weapon = weapon;
            _ammoCreator = ammoCreator;
        }

        public Vector3 GetClosestPosition() => _stack.GetClosestPositionToEject(_carryingPosition.position);

        public async UniTask Carry()
        {
            _animator.SetBool(CarryHash, true);
            _carriedAmmo = _stack.EjectClosest(_carryingPosition.position, _carryingPosition, Vector3.zero, false);
            await UniTask.WaitForSeconds(0.5f);
        }

        public async UniTask Drop()
        {
            _animator.SetBool(CarryHash, false);

            var gunHolder = _weapon.ConnectedCombatManager.Character as StationaryGunHolderCharacter;
            var ammoPrefab = _ammoCreator.GetAmmoPrefab(gunHolder); // AmmoBase prefab
            var visualObject = _carriedAmmo.GameObject; // Stack'ten alınan boş görsel

            await _weapon.LoadWeapon(visualObject, ammoPrefab);
            _carriedAmmo = null;
        }

    }
}