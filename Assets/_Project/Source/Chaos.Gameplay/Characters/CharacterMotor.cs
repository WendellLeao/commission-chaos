using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project
{
    internal sealed class CharacterMotor : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.5f;
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 3f;

        private CharacterController _controller;
        private Vector3 _velocity;

        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 input = ReadMoveInput();
            Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
            float currentSpeed = ReadSprintInput() ? moveSpeed * sprintMultiplier : moveSpeed;

            _controller.Move(moveDirection * currentSpeed * Time.deltaTime);

            if (moveDirection.sqrMagnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (_controller.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            if (_controller.isGrounded && ReadJumpInput())
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed) input.x -= 1f;
                if (keyboard.dKey.isPressed) input.x += 1f;
                if (keyboard.sKey.isPressed) input.y -= 1f;
                if (keyboard.wKey.isPressed) input.y += 1f;
            }

            Gamepad gamepad = Gamepad.current;

            if (gamepad != null)
            {
                input += gamepad.leftStick.ReadValue();
            }

            return Vector2.ClampMagnitude(input, 1f);
        }

        private bool ReadSprintInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null && keyboard.leftShiftKey.isPressed)
            {
                return true;
            }

            Gamepad gamepad = Gamepad.current;

            if (gamepad != null && gamepad.leftStickButton.isPressed)
            {
                return true;
            }

            return false;
        }

        private bool ReadJumpInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            {
                return true;
            }

            Gamepad gamepad = Gamepad.current;

            if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }
    }
}
