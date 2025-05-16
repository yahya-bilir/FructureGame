using System;
using PropertySystem;
using UnityEngine;

namespace WeaponSystem
{
    public class PlayerWeaponManager : CharacterWeaponManager, IDisposable
    {
        private readonly CharacterPropertyManager _characterPropertyManager;

        public PlayerWeaponManager(Transform weaponEquippingField, CharacterPropertyManager characterPropertyManager) : base(weaponEquippingField)
        {
            _characterPropertyManager = characterPropertyManager;
            //todo evente subscribe ol
        }
        
        //todo burada UI tetiklendiğinde gerçekleşecek event ile çalış, Damage'i artır veya yeni ürün spawn et:
        public void UpgradeWeaponDamage()
        {
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Damage); 
            var newDamage = damageData.TemporaryValue + _activeWeapon.WeaponSo.DamageIncrementOnEachUpgrade;
            _activeWeapon.SetNewDamage(newDamage);
            _characterPropertyManager.SetProperty(PropertyQuery.Damage, newDamage);
        }

        public void Dispose()
        {
            
        }
    }
}