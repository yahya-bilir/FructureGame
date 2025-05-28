using System;
using System.Collections.Generic;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters.Player
{
    public class PlayerMovement : IDisposable
    {
        private DynamicJoystick _joystick;
        private readonly Rigidbody2D _rb;
        private readonly PropertyData _speedData;
        private readonly Transform _transform;
        private readonly Transform _meleeWeaponField;
        private readonly IObjectResolver _resolver;
        private Character _nearestEnemy;
        private IEventBus _eventBus;

        public PlayerMovement(Rigidbody2D rb, PropertyData speedData, Transform transform, Transform meleeWeaponField)
        {
            _rb = rb;
            _speedData = speedData;
            _transform = transform;
            _meleeWeaponField = meleeWeaponField;
        }

        [Inject]
        private void Inject(DynamicJoystick joystick, IEventBus eventBus)
        {
            _joystick = joystick;
            _eventBus = eventBus;
            _eventBus.Subscribe<OnNearbyEnemyFoundEvent>(OnEnemyFound);
        }

        private void OnEnemyFound(OnNearbyEnemyFoundEvent eventData)
        {
            _nearestEnemy = eventData.FoundEnemy;
        }

        public void Tick()
        {
            var moveInput = new Vector2(_joystick.Horizontal, _joystick.Vertical);
            _rb.linearVelocity = moveInput.normalized * _speedData.TemporaryValue;

            Vector2? facingDirection = null;

            if (_nearestEnemy != null && !_nearestEnemy.IsCharacterDead)
            {
                var directionToEnemy = (_nearestEnemy.transform.position - _transform.position).normalized;
                facingDirection = directionToEnemy;

                if (directionToEnemy.x > 0.1f)
                    _transform.localEulerAngles = new Vector3(0, 0, 0); // Sağa bak
                else if (directionToEnemy.x < -0.1f)
                    _transform.localEulerAngles = new Vector3(0, 180, 0); // Sola bak
            }
            else if (moveInput != Vector2.zero)
            {
                _rb.simulated = true; 
                facingDirection = moveInput;

                if (moveInput.x > 0.1f)
                    _transform.localEulerAngles = new Vector3(0, 0, 0); // Sağa bak
                else if (moveInput.x < -0.1f)
                    _transform.localEulerAngles = new Vector3(0, 180, 0); // Sola bak
            }
            else
            {
                _rb.simulated = false; 
            }
            // Melee weapon yönlendirme
            if (facingDirection.HasValue)
            {
                var angle = Mathf.Atan2(facingDirection.Value.y, facingDirection.Value.x) * Mathf.Rad2Deg;

                // Eğer sprite yukarı bakıyorsa, +90f gerekebilir. Aksi halde kaldır.
                //angle -= 90f;

                var targetRotation = Quaternion.Euler(0, 0, angle);
                _meleeWeaponField.rotation = Quaternion.Lerp(_meleeWeaponField.rotation, targetRotation, Time.deltaTime * 15f);

            }
        }
        public void Dispose()
        {
            _eventBus.Unsubscribe<OnNearbyEnemyFoundEvent>(OnEnemyFound);
            _resolver?.Dispose();
        }
    }
}