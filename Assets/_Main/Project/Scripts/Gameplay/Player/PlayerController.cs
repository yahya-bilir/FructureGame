using Characters;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerController : Character
    {
        private DynamicJoystick _joystick;
        private PlayerJoystickMovement _playerJoystickMovement;
        
        [Inject]
        protected void Inject(DynamicJoystick joystick)
        {
            _joystick = joystick;
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