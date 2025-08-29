using BasicStackSystem;
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
        private readonly BasicStack _stack;
        private readonly Animator _animator;
        private readonly RangedWeaponWithExternalAmmo _weapon;
        private IStackable _carriedAmmo;
        public bool IsCarrying => _carriedAmmo != null;
        public CarryingController(Transform carryingPosition, BasicStack stack, Animator animator,
            RangedWeaponWithExternalAmmo weapon)
        {
            _carryingPosition = carryingPosition;
            _stack = stack;
            _animator = animator;
            _weapon = weapon;
        }

        public async UniTask Carry()
        {
            _animator.SetBool(CarryHash, true);
            _carriedAmmo = _stack.EjectLastTo(_carryingPosition, Vector3.zero, false);
            await UniTask.WaitForSeconds(0.5f);
        }

        public async UniTask Drop()
        {
            _animator.SetBool(CarryHash, false);
            await _weapon.LoadWeapon(_carriedAmmo.GameObject.GetComponent<AmmoBase>());
            _carriedAmmo =  null;
        }
    }
}