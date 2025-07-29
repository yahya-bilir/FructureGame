using Characters;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace WeaponSystem.Managers
{
    public class CharacterWeaponManager
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly Transform _weaponEquippingField;
        private IObjectResolver _resolver;
        private readonly ObjectWithDamage _weaponPrefabToSpawn;
        public ObjectWithDamage SpawnedWeapon { get; private set; }
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
            if (SpawnedWeapon != null)
            {
                GameObject.Destroy(SpawnedWeapon);
            }
        }


        private void SpawnWeapon(ObjectWithDamage weapon, Transform spawnField)
        {
            if(weapon == null) return;
            SpawnedWeapon = GameObject.Instantiate(weapon, spawnField.position, Quaternion.identity, spawnField);
            SpawnedWeapon.transform.localEulerAngles = Vector3.zero;
            SpawnedWeapon.Initialize(_characterCombatManager, _characterPropertyManager.GetProperty(PropertyQuery.Damage).TemporaryValue);
            _resolver.InjectGameObject(SpawnedWeapon.gameObject);
        }
    }
}