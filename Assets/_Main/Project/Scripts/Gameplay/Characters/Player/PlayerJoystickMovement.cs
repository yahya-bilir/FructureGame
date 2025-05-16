using PropertySystem;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerJoystickMovement
    {
        private DynamicJoystick _joystick;
        private Rigidbody2D _rb;
        private PropertyData _speedData;
        private Transform _transform;

        public PlayerJoystickMovement(Rigidbody2D rb, DynamicJoystick joystick, PropertyData speedData, Transform transform)
        {
            _rb = rb;
            _joystick = joystick;
            _speedData = speedData;
            _transform = transform;
        }
        
        public void Tick()
        {
            var moveInput = new Vector2(_joystick.Horizontal, _joystick.Vertical);
            _rb.linearVelocity = moveInput.normalized * _speedData.TemporaryValue;

            if (moveInput != Vector2.zero)
            {
                if (moveInput.x > 0)
                    _transform.localScale = new Vector3(1, 1, 1);
                else if (moveInput.x < 0)
                    _transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}