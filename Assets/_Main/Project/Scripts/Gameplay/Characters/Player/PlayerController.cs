using DataSave.Runtime;
using PropertySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using WeaponSystem.Managers;

namespace Characters.Player
{
    public class PlayerController : Character
    {
        [SerializeField] private Transform weaponCreationPoint;
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform projectileWeaponCreationPoint;
        [SerializeField] private ParticleSystem onWeaponUpgradedVFX;
        
        private DynamicJoystick _joystick;
        private PlayerMovement _playerMovement;
        private PlayerWeaponManager _weaponManager;
        private GameData _gameData;
        private IObjectResolver _resolver;
        [Header("Debug")] 
        [SerializeField]private MeleeWeapon meleeWeapon;
        
        
        [Inject]
        protected void Inject(GameData gameData, IObjectResolver resolver)
        {
            _gameData = gameData;
            _resolver = resolver;
        }

        protected override void Awake()
        {
            base.Awake();
            _weaponManager = new PlayerWeaponManager(pivot, projectileWeaponCreationPoint, CharacterPropertyManager, CharacterCombatManager, onWeaponUpgradedVFX);
        }

        protected override void Start()
        {
            base.Start();
            var speed = CharacterPropertyManager.GetProperty(PropertyQuery.Speed);
            _playerMovement = new PlayerMovement(GetComponent<Rigidbody2D>(), speed, model.transform, weaponCreationPoint);
            _resolver.Inject(_playerMovement);
            _resolver.Inject(_weaponManager);
            
            //EquipWeapon();
        }

        [Button]
        private void EquipWeapon()
        {
            _weaponManager.SpawnWeapon(meleeWeapon, weaponCreationPoint);
            meleeWeapon.SetNewDamage(1);
        }

        private void Update()
        {
            _playerMovement.Tick();

            // if (Input.GetKeyDown(KeyCode.LeftControl))
            // {
            //     EquipWeapon();
            // }
        }
    }
}