using Cinemachine;
using System.Collections;
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
    public CinemachineVirtualCamera virtualCamera; // Referencia a la Cinemachine Virtual Camera
    public Camera playerCamera;
    public float interactRange = 3f; // Distancia máxima de interacción
    public LayerMask interactableLayer; // Opcional: filtrar solo objetos interactuables

    private CharacterController controller;
    private Vector2 moveInput;
    private float currentSpeed;
    private Vector3 velocity;

    public enum PlayerState
    {
        Wait,
        Walking,
        Running,
        Crouching,
        Hiding,
        Block,
        Dead
    }

    public PlayerState currentState;

    // Bool para activar/desactivar cada estado
    [Header("State Control")]
    public bool canWalk = true;
    public bool canRun = true;
    public bool canCrouch = true;
    public bool blockPlayer = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentState = PlayerState.Walking;
    }

    private void OnEnable()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        playerInput.actions["Run"].started += ctx => StartRunning();
        playerInput.actions["Run"].canceled += ctx => StopRunning();

        playerInput.actions["Crouch"].started += ctx => ToggleCrouch();
        playerInput.actions["Jump"].started += ctx => Jump();

        playerInput.actions["Interact"].started += ctx => Interact();

        playerInput.actions["Test1"].started += ctx => EnterDeadState();
        playerInput.actions["Test2"].started += ctx => ExitDeadState();

    }

    private void Update()
    {
        BlockPlayer();
        Move();
        RotatePlayer();
        ApplyGravity();
        HandleCamera();

        Debug.Log(currentState.ToString());
    }

    private void HandleCamera()
    {
        if (currentState == PlayerState.Dead)
        {
            // Desactivar el seguimiento del jugador bloqueando la cámara
            //virtualCamera.Follow = null; // Deja de seguir al jugador
            //virtualCamera.m_HorizontalAxis.m_MaxSpeed = 0f;
            //virtualCamera.m_VerticalAxis.m_MaxSpeed = 0f;

        }
        else
        {
            // Restaurar la cámara si no está en estado Dead
            if (virtualCamera != null)
            {
                //virtualCamera.m_HorizontalAxis.m_MaxSpeed = 300f;
                //virtualCamera.m_VerticalAxis.m_MaxSpeed = 300f;
            }
        }
    }

    public void EnterDeadState()
    {
        currentState = PlayerState.Dead;
        //blockPlayer = true; // Bloquea al jugador
        HandleCamera();     // Desactiva el movimiento de la cámara
    }

    public void ExitDeadState()
    {
        currentState = PlayerState.Walking; // O el estado deseado
        //blockPlayer = false; // Vuelve a activar al jugador
        HandleCamera();      // Reactiva el movimiento de la cámara
    }


    private void Move()
    {
        if (currentState == PlayerState.Dead)
        {
            //currentSpeed = 0f;
            return;
        }

        // Si el estado actual es Block, no se mueve
        if (currentState == PlayerState.Block)
        {
            currentSpeed = 0f;
            return;
        }

        // Si el estado actual es Hiding, no se mueve
        if (currentState == PlayerState.Hiding)
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

    public void EnterHiding(Vector3 insidePosition)
    {
        currentState = PlayerState.Hiding;
        controller.height = normalHeight;
        StartCoroutine(MoveToPosition(insidePosition)); // Iniciar la interpolación hacia el interior del armario
    }

    public void ExitHiding(Vector3 outsidePosition)
    {
        currentState = PlayerState.Walking;
        StartCoroutine(MoveToPosition(outsidePosition)); // Iniciar la interpolación hacia el exterior del armario
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float timeToMove = 1.0f; // Duración del movimiento
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Asegurar que la posición final sea exacta
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
        if (canCrouch && currentState != PlayerState.Block && currentState != PlayerState.Hiding)
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

    // Método para interactuar con objetos
    private void Interact()
    {
        // Lanza un rayo desde la posición de la cámara hacia adelante
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact(); // Llama al método Interact si el objeto tiene IInteractable
            }
        }
    }

}
