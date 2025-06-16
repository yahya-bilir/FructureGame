using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using DataSave;
using DataSave.Runtime;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using VContainer;
using WeaponSystem.RangedWeapons;

namespace WeaponSystem.Managers
{
    public class CharacterWeaponManager
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly ParticleSystem _onWeaponUpgradedVFX;
        private readonly Transform _meleeWeaponEquippingField;
        private readonly Transform _rangedWeaponEquippingField;
        private IObjectResolver _resolver;
        private ObjectWithDamage _currentWeapon;

        public CharacterWeaponManager(Transform meleeWeaponEquippingField, Transform rangedWeaponEquippingField,
            CharacterPropertyManager characterPropertyManager, CharacterCombatManager characterCombatManager)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _meleeWeaponEquippingField = meleeWeaponEquippingField;
            _rangedWeaponEquippingField = rangedWeaponEquippingField;
        }

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            InitializeWeapon();
        }
        

        private void InitializeWeapon()
        {
            var weapon = _currentWeapon;
            var transform = weapon.ObjectUIIdentifierSo is RangedWeaponSO ? _rangedWeaponEquippingField : _meleeWeaponEquippingField;
            SpawnWeapon(weapon, transform);
            if(_onWeaponUpgradedVFX != null) _onWeaponUpgradedVFX.Play();
        }
        
        private void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            var newWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.Initialize(_characterCombatManager);
            _resolver.Inject(newWeapon);
        }
    }
}