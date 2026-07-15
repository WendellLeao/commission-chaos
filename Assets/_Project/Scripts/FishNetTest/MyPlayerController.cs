using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    
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
        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
 
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) input.x -= 1f;
            if (keyboard.dKey.isPressed) input.x += 1f;
            if (keyboard.sKey.isPressed) input.y -= 1f;
            if (keyboard.wKey.isPressed) input.y += 1f;
        }
 
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _controller.Move(move * moveSpeed * Time.deltaTime);
 
        if (_controller.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }
 
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
