using System.Collections.Generic;
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
        private Transform _weaponPoint;
        private Vector3 _initialScale;

        public PlayerJoystickMovement(Rigidbody2D rb, DynamicJoystick joystick, PropertyData speedData,
            Transform transform, Transform weaponPoint)
        {
            _rb = rb;
            _joystick = joystick;
            _speedData = speedData;
            _transform = transform;
            _weaponPoint = weaponPoint;
            _initialScale = _transform.localScale;
        }
        
        public void Tick()
        {
            var moveInput = new Vector2(_joystick.Horizontal, _joystick.Vertical);
            _rb.linearVelocity = moveInput.normalized * _speedData.TemporaryValue;
            if (moveInput != Vector2.zero)
            {
                _transform.localScale = new Vector3(moveInput.x >= 0 ? _initialScale.x : 0 - _initialScale.x, _initialScale.y, _initialScale.z);
                var angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(0, 0, angle);
                _weaponPoint.rotation = Quaternion.Lerp(_weaponPoint.rotation, targetRotation, Time.deltaTime * 15f);

            }
        }

    }
}