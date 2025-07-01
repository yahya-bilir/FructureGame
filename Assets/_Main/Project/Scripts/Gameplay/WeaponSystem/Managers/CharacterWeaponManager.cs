using Characters;
using EventBusses;
using Events;
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
        private readonly ObjectWithDamage _weaponPrefabToSpawn;
        private ObjectWithDamage _spawnedWeapon;
        private readonly Character _character;
        private IEventBus _eventBus;

        public CharacterWeaponManager(Transform weaponEquippingField,
            CharacterPropertyManager characterPropertyManager,
            CharacterCombatManager characterCombatManager,
            ObjectWithDamage weaponPrefabToSpawn, Character character)
        {
            _characterPropertyManager = characterPropertyManager;
            _characterCombatManager = characterCombatManager;
            _weaponEquippingField = weaponEquippingField;
            _weaponPrefabToSpawn = weaponPrefabToSpawn;
            _character = character;
        }

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            SpawnWeapon(_weaponPrefabToSpawn, _weaponEquippingField);
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
        }

        private void OnCharacterDied(OnCharacterDied data)
        {
            if(data.Character != _character) return;
            if (_spawnedWeapon != null)
            {
                GameObject.Destroy(_spawnedWeapon);
            }
        }


        private void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            _spawnedWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            _spawnedWeapon.transform.localEulerAngles = Vector3.zero;
            _spawnedWeapon.Initialize(_characterCombatManager, _characterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
            _resolver.Inject(_spawnedWeapon);
        }
    }
}