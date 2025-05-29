using System;
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
    public class PlayerWeaponManager : CharacterWeaponManager, IDisposable
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly ParticleSystem _onWeaponUpgradedVFX;
        private readonly Transform _meleeWeaponEquippingField;
        private readonly Transform _rangedWeaponEquippingField;
        private GameData _gameData;
        private GameDatabase _gameDatabase;
        private IEventBus _eventBus;
        private IObjectResolver _resolver;

        public PlayerWeaponManager(Transform meleeWeaponEquippingField, Transform rangedWeaponEquippingField,
            CharacterPropertyManager characterPropertyManager, CharacterCombatManager characterCombatManager,
            ParticleSystem onWeaponUpgradedVFX)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _onWeaponUpgradedVFX = onWeaponUpgradedVFX;
            _meleeWeaponEquippingField = meleeWeaponEquippingField;
            _rangedWeaponEquippingField = rangedWeaponEquippingField;
        }

        [Inject]
        private void Inject(GameData gameData, GameDatabase gameDatabase, IEventBus eventBus, IObjectResolver resolver)
        {
            _gameData = gameData;
            _gameDatabase = gameDatabase;
            _eventBus = eventBus;
            _resolver = resolver;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnUpgradeButtonPressed>(UpgradePlayerDamage);
            InitiateWeapon().Forget();
        }

        private void UpgradePlayerDamage(OnUpgradeButtonPressed eventData)
        {
            UpgradeableWeapon weapon = (UpgradeableWeapon)Weapons.Find(i => i is UpgradeableWeapon);

            if (weapon == null) return;
            
            var damageData = _characterPropertyManager.GetProperty(PropertyQuery.Damage);

            var upgradeableData = weapon.ObjectUIIdentifierSo as UpgreadableWeaponSO;
            var newDamage = damageData.TemporaryValue + upgradeableData.DamageIncrementOnEachUpgrade;

            weapon.SetNewDamage(newDamage);

            var stage = _gameData.EnhanceButtonData.TemporaryButtonClickedCount %
                        _gameDatabase.WeaponDatabase.WeaponStages.Count;
            var weaponNumber = _gameData.EnhanceButtonData.TemporaryButtonClickedCount /
                               _gameDatabase.WeaponDatabase.WeaponStages.Count;

            _characterPropertyManager.SetProperty(PropertyQuery.Damage, newDamage);

            var upgradeData = new OnWeaponUpgraded(

                _gameDatabase.WeaponDatabase.WeaponStages[stage],
                (int)newDamage,
                (int)damageData.TemporaryValue,
                weapon.CurrentAttackInterval,
                _gameData.EnhanceButtonData.TemporaryButtonClickedCount + 1,
                _gameDatabase.WeaponDatabase.Weapons[weaponNumber].ObjectUIIdentifierSo
            );

            _eventBus.Publish(upgradeData);
            
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
            if(_onWeaponUpgradedVFX != null) _onWeaponUpgradedVFX.Play();
        }

        private async UniTask InitiateWeapon()
        {
            await UniTask.WaitForSeconds(0.25f);
            var weaponDatabase = _gameDatabase.WeaponDatabase;
            var weaponStages = weaponDatabase.WeaponStages;
            var enhanceButtonData = _gameData.EnhanceButtonData;
            
            _gameData.EnhanceButtonData.TemporaryButtonClickedCount = enhanceButtonData.ButtonClickedCount;
            var weaponNumber = enhanceButtonData.TemporaryButtonClickedCount / weaponStages.Count;
            
            ReplaceWeapon(weaponNumber);
        }
        public void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            var newWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.Initialize(_characterCombatManager);
            Weapons.Add(newWeapon);
            _resolver.Inject(newWeapon);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnUpgradeButtonPressed>(UpgradePlayerDamage);
        }
    }
}