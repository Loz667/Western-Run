using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]   
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float initialPlayerSpeed = 4f;
    [SerializeField] private float maxPlayerSpeed = 30f;
    [SerializeField] private float playerSpeedIncreaseRate = .1f;
    [SerializeField] private float playerJumpHeight = 1.0f;
    [SerializeField] private float initialGravityValue = -9.81f;
    [SerializeField] private LayerMask groundLayer;

    private float playerSpeed;
    private float currentGravity;
    private Vector3 playerVelocity;
    private Vector3 movementDirection = Vector3.forward;

    private PlayerInput input;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController controller;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        turnAction = input.actions["Turn"];
        jumpAction = input.actions["Jump"];
        slideAction = input.actions["Slide"];
    }

    private void OnEnable()
    {
        turnAction.performed += PlayerTurn;
        jumpAction.performed += PlayerJump;
        slideAction.performed += PlayerSlide;
    }

    private void OnDisable()
    {
        turnAction.performed -= PlayerTurn;
        jumpAction.performed -= PlayerJump;
        slideAction.performed -= PlayerSlide;
    }

    private void Start()
    {
        playerSpeed = initialPlayerSpeed;
        currentGravity = initialGravityValue;
    }

    private void PlayerTurn(InputAction.CallbackContext context)
    {

    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(playerJumpHeight * currentGravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {

    }

    private void Update()
    {
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        playerVelocity.y += currentGravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private bool IsGrounded(float length = .2f)
    {
        Vector3 firstRaycastOrigin  = transform.position;
        firstRaycastOrigin.y -= controller.height / 2f;
        firstRaycastOrigin.y += .1f;

        Vector3 secondRaycastOrigin = firstRaycastOrigin;
        firstRaycastOrigin -= transform.forward * .2f;
        secondRaycastOrigin += transform.forward * .2f;

        if (Physics.Raycast(firstRaycastOrigin, Vector3.down, out RaycastHit hit, length, groundLayer) || 
            Physics.Raycast(secondRaycastOrigin, Vector3.down, out RaycastHit hit2, length, groundLayer))
        {
            return true;
        }
        return false;
    }
}
