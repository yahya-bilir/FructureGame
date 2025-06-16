using Characters;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace WeaponSystem.Managers
{
    public class CharacterWeaponManager
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly Transform _weaponEquippingField;
        private IObjectResolver _resolver;
        private readonly ObjectWithDamage _currentWeapon;

        public CharacterWeaponManager(Transform weaponEquippingField,
            CharacterPropertyManager characterPropertyManager, 
            CharacterCombatManager characterCombatManager,
            ObjectWithDamage currentWeapon)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _weaponEquippingField = weaponEquippingField;
            _currentWeapon = currentWeapon;
        }

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            SpawnWeapon(_currentWeapon, _weaponEquippingField);
        }
        
        
        private void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            var newWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.Initialize(_characterCombatManager, _characterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
            _resolver.Inject(newWeapon);
        }
    }
}