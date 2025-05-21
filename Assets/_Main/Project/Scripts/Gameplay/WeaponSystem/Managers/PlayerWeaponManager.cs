using System;
using Characters;
using PropertySystem;
using UnityEngine;
using WeaponSystem.RangedWeapons;

namespace WeaponSystem.Managers
{
    public class PlayerWeaponManager : CharacterWeaponManager, IDisposable
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly Transform _meleeWeaponEquippingField;
        private readonly Transform _rangedWeaponEquippingField;
        public PlayerWeaponManager(Transform meleeWeaponEquippingField, Transform rangedWeaponEquippingField, CharacterPropertyManager characterPropertyManager, CharacterCombatManager characterCombatManager)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _meleeWeaponEquippingField = meleeWeaponEquippingField;
            _rangedWeaponEquippingField = rangedWeaponEquippingField;
            //todo evente subscribe ol
        }
        
        //todo burada UI tetiklendiğinde gerçekleşecek event ile çalış, Damage'i artır veya yeni ürün spawn et:
        public void UpgradePlayerDamage(ObjectWithDamage weapon)
        {
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Damage);
            
            var upgradeableData = weapon.ObjectUIIdentifierSo as UpgradableWeaponSO;
            var newDamage = damageData.TemporaryValue + upgradeableData.DamageIncrementOnEachUpgrade;
            
            weapon.SetNewDamage(newDamage);
            _characterPropertyManager.SetProperty(PropertyQuery.Damage, newDamage);
            
            if (weapon is RangedWeapon ranged)
            {
                ranged.UpgradeAttackInterval();
            }
            
            //todo burada eğer gerekli miktarda upgrade yapıldıysa yeni ürün spawn et
        }

        public override void ReplaceWeapon(ObjectWithDamage weapon)
        {
            if(Weapons.Contains(weapon)) GameObject.Destroy(weapon.gameObject);
            var transform = weapon.ObjectUIIdentifierSo is RangedWeaponSO ? _rangedWeaponEquippingField : _meleeWeaponEquippingField;
            SpawnWeapon(weapon, transform);
        }

        private ObjectWithDamage SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            var newWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.Initialize(_characterCombatManager);
            return newWeapon;
        }

        public void Dispose()
        {
            
        }
    }
}