using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1f;
    public float crouchHeight = 0.5f;
    public float normalHeight = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Look Settings")]
    public Camera playerCamera;

    private CharacterController controller;
    private Vector2 moveInput;

    private float currentSpeed;
    private Vector3 velocity;

    private enum PlayerState
    {
        Walking,
        Running,
        Crouching,
        Block, // Estado que impide movimiento
        Wait // Estado cuando no se está moviendo
    }

    private PlayerState currentState;

    // Bool para activar/desactivar cada estado
    [Header("State Control")]
    public bool canWalk = true;
    public bool canRun = true;
    public bool canCrouch = true;
    public bool blockPlayer = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentState = PlayerState.Walking; // Estado inicial
    }

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        playerInput.actions["Run"].started += ctx => StartRunning();
        playerInput.actions["Run"].canceled += ctx => StopRunning();

        playerInput.actions["Crouch"].started += ctx => ToggleCrouch();
        playerInput.actions["Jump"].started += ctx => Jump();
    }

    private void Update()
    {
        BlockPlayer();
        Move();
        RotatePlayer();
        ApplyGravity();

        Debug.Log(currentState.ToString());
    }

    private void Move()
    {
        // Si el estado actual es Block, no se mueve
        if (currentState == PlayerState.Block)
        {
            currentSpeed = 0f;
            return;
        }

        // Cambiar a Wait si no se está moviendo
        if (moveInput == Vector2.zero && currentState != PlayerState.Crouching)
        {
            currentState = PlayerState.Wait;
            currentSpeed = 0f;
            return;
        }
        else if (currentState == PlayerState.Wait)
        {
            currentState = PlayerState.Walking; // Cambia a Walking si empieza a moverse
        }

        float targetSpeed = currentState == PlayerState.Running ? runSpeed : (currentState == PlayerState.Crouching ? crouchSpeed : walkSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        Quaternion cameraRotation = playerCamera.transform.rotation;
        transform.rotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        // No puede saltar si está agachado o en estado Block
        if (controller.isGrounded && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ToggleCrouch()
    {
        // Solo puede agacharse si está habilitado y no está en estado Block
        if (canCrouch && currentState != PlayerState.Block)
        {
            if (currentState == PlayerState.Crouching)
            {
                controller.height = normalHeight;
                currentState = PlayerState.Walking; // Cambiar a estado de andar
            }
            else
            {
                controller.height = crouchHeight;
                currentState = PlayerState.Crouching; // Cambiar a estado agachado
            }
        }
    }

    private void StartRunning()
    {
        // Solo puede correr si está habilitado y no está en estado Block
        if (canRun && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            currentState = PlayerState.Running;
        }
    }

    private void StopRunning()
    {
        // Solo puede detenerse de correr si está habilitado y no está en estado Block
        if (canRun && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            currentState = PlayerState.Walking;
        }
    }

    private void BlockPlayer()
    {
        // Cambiar a estado Block si el booleano está activado
        if (blockPlayer)
        {
            currentState = PlayerState.Block;
        }
        else if (currentState == PlayerState.Block)
        {
            currentState = PlayerState.Walking; // Cambiar a Walking si se desactiva el bloqueo
        }
    }
}
