using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace WesternRun.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float initialPlayerSpeed = 4f;
        [SerializeField] private float maxPlayerSpeed = 30f;
        [SerializeField] private float playerSpeedIncreaseRate = .1f;
        [SerializeField] private float playerJumpHeight = 1.0f;
        [SerializeField] private float initialGravityValue = -9.81f;
        [SerializeField] private LayerMask groundLayer, turnLayer;

        private float playerSpeed;
        private float currentGravity;
        private Vector3 playerVelocity;
        private Vector3 movementDirection = Vector3.forward;

        private PlayerInput input;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;

        private CharacterController controller;

        [SerializeField] private UnityEvent<Vector3> turnEvent;

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
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue)
                return;
            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
            turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        private void Turn(float value, Vector3 position)
        {
            Vector3 tempPlayerPosition = new Vector3(position.x, transform.position.y, position.z);
            controller.enabled = false;
            transform.position = tempPlayerPosition;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * value, 0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;
        }

        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] turnCollider = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (turnCollider.Length != 0)
            {
                Tile tile = turnCollider[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                if ((type == TileType.Left && turnValue == -1) ||
                    (type == TileType.Right && turnValue == 1) ||
                    (type == TileType.Sideways))
                {
                    return tile.pivot.position;
                }
            }
            return null;
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
            Vector3 firstRaycastOrigin = transform.position;
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
}