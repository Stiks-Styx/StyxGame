using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 velocity;
    private bool isCursorLocked = true;

    private InputSystem_Actions inputActions;
    private Vector3 _input;
    private CharacterController characterController;
    private bool isGrounded = true;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {

        isGrounded = characterController.isGrounded;
        GetInput();
        RotatePlayer();
        MovePlayer();
        LockScreen();
    }

    private void GetInput()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
    }

    private void RotatePlayer()
    {
        if (_input.x != 0)
        {
            float rotation = _input.x * rotationSpeed * 2f * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }

    private void MovePlayer()
    {
        Vector3 forwardMovement = transform.forward * _input.z * speed * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset velocity when grounded
        }

        // Handle jumping
        if (inputActions.Player.Jump.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine movement and gravity
        Vector3 movement = forwardMovement + velocity * Time.deltaTime;
        characterController.Move(movement);
    }

    private void LockScreen()
    {
        if (inputActions.Player.Esc.triggered)
        {
            isCursorLocked = !isCursorLocked;
        }

        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !isCursorLocked;
    }
}