using PropertySystem;
using UnityEngine;
using VContainer;
using WeaponSystem;

namespace Characters.Player
{
    public class PlayerController : Character
    {
        [SerializeField] private Transform weaponCreationPoint;
        
        private DynamicJoystick _joystick;
        private PlayerJoystickMovement _playerJoystickMovement;
        private PlayerWeaponManager _weaponManager;

        [Inject]
        protected void Inject(DynamicJoystick joystick)
        {
            _joystick = joystick;
        }

        protected override void Awake()
        {
            base.Awake();
            CharacterCombatManager = new PlayerCombatManager(this, CharacterPropertyManager, CharacterDataHolder);
            _weaponManager = new PlayerWeaponManager(weaponCreationPoint, CharacterPropertyManager);
        }

        private void Start()
        {
            var speed = CharacterPropertyManager.GetProperty(PropertyQuery.Speed);
            _playerJoystickMovement = new PlayerJoystickMovement(GetComponent<Rigidbody2D>(), _joystick,
                speed, transform);
            
        }

        private void Update()
        {
            _playerJoystickMovement.Tick();
        }
    }
}