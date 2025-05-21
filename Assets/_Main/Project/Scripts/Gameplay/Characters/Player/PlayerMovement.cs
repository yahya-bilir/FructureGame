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
        private readonly IObjectResolver _resolver;
        private readonly Vector3 _initialScale;
        private Character _nearestEnemy;
        private IEventBus _eventBus;

        public PlayerMovement(Rigidbody2D rb, PropertyData speedData, Transform transform)
        {
            _rb = rb;
            _speedData = speedData;
            _transform = transform;
            _initialScale = _transform.localScale;
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

            if (_nearestEnemy != null && !_nearestEnemy.IsCharacterDead)
            {
                var directionToEnemy = (_nearestEnemy.transform.position - _transform.position).normalized;

                _transform.localScale = new Vector3(
                    directionToEnemy.x >= 0 ? _initialScale.x : -_initialScale.x,
                    _initialScale.y,
                    _initialScale.z);
            }
            else if (moveInput != Vector2.zero)
            {
                _transform.localScale = new Vector3(
                    moveInput.x >= 0 ? _initialScale.x : -_initialScale.x,
                    _initialScale.y,
                    _initialScale.z);
            }
            
            
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnNearbyEnemyFoundEvent>(OnEnemyFound);
            _resolver?.Dispose();
        }
    }
}