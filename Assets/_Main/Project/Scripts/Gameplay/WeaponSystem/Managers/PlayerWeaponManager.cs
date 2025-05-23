using System;
using System.Linq;
using Characters;
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
    public class PlayerWeaponManager : CharacterWeaponManager, IDisposable
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly Transform _meleeWeaponEquippingField;
        private readonly Transform _rangedWeaponEquippingField;
        private GameData _gameData;
        private GameDatabase _gameDatabase;
        private IEventBus _eventBus;
        
        public PlayerWeaponManager(Transform meleeWeaponEquippingField, Transform rangedWeaponEquippingField, CharacterPropertyManager characterPropertyManager, CharacterCombatManager characterCombatManager)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _meleeWeaponEquippingField = meleeWeaponEquippingField;
            _rangedWeaponEquippingField = rangedWeaponEquippingField;
        }

        [Inject]
        private void Inject(GameData gameData, GameDatabase gameDatabase, IEventBus eventBus)
        {
            _gameData = gameData;
            _gameDatabase = gameDatabase;
            _eventBus = eventBus;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnUpgradeButtonPressed>(UpgradePlayerDamage);
            InitiateWeapon();
        }

        private void UpgradePlayerDamage(OnUpgradeButtonPressed eventData)
        {
            RangedWeapon weapon = (RangedWeapon)Weapons.Find(i => i is RangedWeapon);

            if (weapon == null) return;

            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Damage);

            var upgradeableData = weapon.ObjectUIIdentifierSo as RangedWeaponSO;
            var newDamage = damageData.TemporaryValue + upgradeableData.DamageIncrementOnEachUpgrade;

            weapon.SetNewDamage(newDamage);

            var stage = _gameData.EnhanceButtonData.TemporaryButtonClickedCount %
                        _gameDatabase.WeaponDatabase.WeaponStages.Count;
            var weaponNumber = _gameData.EnhanceButtonData.TemporaryButtonClickedCount /
                               _gameDatabase.WeaponDatabase.WeaponStages.Count;
            
            var upgradeData = new OnWeaponUpgraded(

                _gameDatabase.WeaponDatabase.WeaponStages[stage],
                (int)newDamage,
                (int)damageData.TemporaryValue,
                weapon.CurrentAttackInterval,
                _gameData.EnhanceButtonData.TemporaryButtonClickedCount,
                upgradeableData
            );

            _eventBus.Publish(upgradeData);
            
            _characterPropertyManager.SetProperty(PropertyQuery.Damage, newDamage);

            if (eventData == null) return;
            if (stage == 0)
            {
                ReplaceWeapon(weaponNumber);
            }
        }

        public override void ReplaceWeapon(int weaponNumber)
        {
            var weaponDatabase = _gameDatabase.WeaponDatabase.Weapons;
            if (weaponNumber > 0)
            {
                var currentWeaponNumber = weaponNumber - 1;
                var currentWeaponIdentifier = weaponDatabase[currentWeaponNumber].ObjectUIIdentifierSo;
                if(Weapons.Any(i => i.ObjectUIIdentifierSo == currentWeaponIdentifier))
                {
                    var currentWeapon = Weapons.First(i => i.ObjectUIIdentifierSo == currentWeaponIdentifier);
                    Weapons.Remove(currentWeapon);
                    GameObject.Destroy(currentWeapon.gameObject);
                }
            }

            var weapon = weaponDatabase[weaponNumber];
            var transform = weapon.ObjectUIIdentifierSo is RangedWeaponSO ? _rangedWeaponEquippingField : _meleeWeaponEquippingField;
            SpawnWeapon(weapon, transform);
            UpgradePlayerDamage(null);

        }

        private void InitiateWeapon()
        {
            var weapons = _gameDatabase.WeaponDatabase.Weapons;
            var weaponStages = _gameDatabase.WeaponDatabase.WeaponStages;
            var enhanceButtonData = _gameData.EnhanceButtonData;
            
            _gameData.EnhanceButtonData.TemporaryButtonClickedCount = enhanceButtonData.ButtonClickedCount;
            var weaponNumber = enhanceButtonData.TemporaryButtonClickedCount / weaponStages.Count;
            var stage = enhanceButtonData.ButtonClickedCount % weaponStages.Count;
            
            ReplaceWeapon(weaponNumber);
            UpgradePlayerDamage(null);
        }
        private void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            var newWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.Initialize(_characterCombatManager);
            Weapons.Add(newWeapon);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnUpgradeButtonPressed>(UpgradePlayerDamage);
        }
    }
}