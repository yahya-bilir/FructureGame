using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerJoystickMovement : MonoBehaviour
    {
        private DynamicJoystick _joystick;
        public float moveSpeed = 5f;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
        [Inject]
        public void Inject(DynamicJoystick joystick)
        {
            _joystick = joystick;
        }
        void Update()
        {
            var moveInput = new Vector2(_joystick.Horizontal, _joystick.Vertical);
            _rb.linearVelocity = moveInput.normalized * moveSpeed;

            if (moveInput != Vector2.zero)
            {
                if (moveInput.x > 0)
                    transform.localScale = new Vector3(1, 1, 1);
                else if (moveInput.x < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}