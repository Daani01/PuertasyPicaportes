using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
    public float mouseSensitivity = 100f; // Sensibilidad de la rotación
    public float maxVerticalAngle = 80f; // Máximo ángulo vertical
    public float minVerticalAngle = -80f; // Mínimo ángulo vertical
    public float rotationSmoothness = 0.1f; // Suavidad de la rotación
    public Camera playerCamera;
    public float interactRange = 3f; // Distancia máxima de interacción
    public LayerMask interactableLayer; // Opcional: filtrar solo objetos interactuables

    [Header("Inventory")]
    public List<IUsable> inventory = new List<IUsable>(); // Lista para almacenar objetos recogidos
    public IUsable selectedObject;


    private CharacterController controller;
    private Vector2 moveInput;

    private Vector2 lookInput; // Input del ratón
    private Vector2 currentRotation; // Rotación actual
    private Vector2 targetRotation; // Rotación objetivo
    private Vector2 rotationVelocity; // Velocidad de la rotación (para suavizado)

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
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor al centro de la pantalla
        Cursor.visible = false; // Esconde el cursor

        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        playerInput.actions["Look"].performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Look"].canceled += ctx => lookInput = Vector2.zero;

        playerInput.actions["Run"].started += ctx => StartRunning();
        playerInput.actions["Run"].canceled += ctx => StopRunning();

        playerInput.actions["Crouch"].started += ctx => ToggleCrouch();
        playerInput.actions["Jump"].started += ctx => Jump();

        playerInput.actions["Interact"].started += ctx => Interact();

        playerInput.actions["SelectObj1"].started += ctx => SelectObj(1);
        playerInput.actions["SelectObj2"].started += ctx => SelectObj(2);

        playerInput.actions["ActivateObj"].started += ctx => ActivateObj();

    }

    private void Update()
    {
        BlockPlayer();
        Move();
        Look();
        RotatePlayer();
        ApplyGravity();
        //HandleCamera();

        //Debug.Log(currentState.ToString());
    }

    private void Look()
    {
        // Calcula la rotación objetivo en función del input del ratón
        targetRotation.x += lookInput.x * mouseSensitivity; // Rotación en el eje Y (horizontal)
        targetRotation.y -= lookInput.y * mouseSensitivity; // Rotación en el eje X (vertical)

        // Limita la rotación vertical
        targetRotation.y = Mathf.Clamp(targetRotation.y, minVerticalAngle, maxVerticalAngle);

        // Interpola suavemente entre la rotación actual y la rotación objetivo
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, rotationSmoothness);

        // Aplica la rotación en el jugador (rotación en el eje Y)
        transform.rotation = Quaternion.Euler(0f, currentRotation.x, 0f);

        // Aplica la rotación en la cámara (rotación en el eje X)
        playerCamera.transform.localRotation = Quaternion.Euler(currentRotation.y, 0f, 0f);
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
            // Primero interactúa con el objeto
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.InteractObj(); // Llama al método Interact si el objeto tiene Interactable
            }

            // Luego verifica si también es usable
            IUsable usable = hit.collider.GetComponent<IUsable>();
            if (usable != null)
            {
                PickUpItem(usable); // Recoger el objeto si es usable
            }
        }
    }


    public void PickUpItem(IUsable usableItem)
    {
        // Verifica si ya tienes un objeto del mismo tipo
        foreach (var item in inventory)
        {
            if (item.GetType() == usableItem.GetType())
            {
                Debug.Log("Ya tienes un objeto de este tipo en el inventario.");
                return;
            }
        }

        if (inventory.Count < 6) // Máximo de 6 objetos
        {
            inventory.Add(usableItem);
            Debug.Log("Objeto añadido al inventario.");
        }
        else
        {
            Debug.Log("Inventario lleno.");
        }
    }


    public void SelectObj(int index)
    {
        if (index > 0 && index <= inventory.Count)
        {
            selectedObject = inventory[index - 1];
            Debug.Log($"Object {index} selected.");
        }
        else
        {
            Debug.Log("Invalid selection.");
        }
    }

    public void ActivateObj()
    {
        if (selectedObject != null)
        {
            selectedObject.Use(); // Activa el objeto seleccionado
        }
        else
        {
            Debug.Log("No object selected.");
        }
    }

}
