using System.Collections.Generic;
using DataSave.Runtime;
using PropertySystem;
using UnityEngine;
using VContainer;
using WeaponSystem;

namespace Characters.Player
{
    public class PlayerController : Character
    {
        [SerializeField] private Transform weaponCreationPoint;
        [SerializeField] private Transform weaponMovingPoint;
        
        private DynamicJoystick _joystick;
        private PlayerJoystickMovement _playerJoystickMovement;
        private PlayerWeaponManager _weaponManager;
        private GameData _gameData;

        [Inject]
        protected void Inject(GameData gameData, DynamicJoystick joystick)
        {
            _joystick = joystick;
            _gameData = gameData;
        }

        protected override void Awake()
        {
            base.Awake();
            _weaponManager = new PlayerWeaponManager(weaponCreationPoint, CharacterPropertyManager);
        }

        protected override void Start()
        {
            base.Start();
            var speed = CharacterPropertyManager.GetProperty(PropertyQuery.Speed);
            _playerJoystickMovement = new PlayerJoystickMovement(GetComponent<Rigidbody2D>(), _joystick,
                speed, model.transform, weaponMovingPoint);
            
        }

        private void Update()
        {
            _playerJoystickMovement.Tick();
        }
    }
}