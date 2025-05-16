using System;
using UnityEngine;

namespace WeaponSystem
{
    public class CharacterWeaponManager
    {
        private Transform _weaponEquippingField;
        protected Weapon _activeWeapon;

        public CharacterWeaponManager(Transform weaponEquippingField)
        {
            _weaponEquippingField = weaponEquippingField;
        }
        
        public virtual void SpawnNewWeapon(Weapon weapon)
        {
            if(_activeWeapon != null) GameObject.Destroy(_activeWeapon.gameObject);
            _activeWeapon = GameObject.Instantiate(weapon, _weaponEquippingField.position, Quaternion.identity, _weaponEquippingField);
        }
    }
}