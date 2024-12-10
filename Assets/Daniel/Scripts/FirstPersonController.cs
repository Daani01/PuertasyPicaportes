using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("HUD")]
    public TMP_Text Text_State;
    public TMP_Text Text_Objets;

    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float crouchHeight;
    public float normalHeight;
    public float gravity;
    public float jumpHeight;

    [Header("Player Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Money Settings")]
    public int coinsCount;

    [Header("Look Settings")]
    public float mouseSensitivity;
    public float maxVerticalAngle;
    public float minVerticalAngle;
    public float rotationSmoothness;
    public Camera playerCamera;
    public float interactRange;
    public LayerMask interactableLayer;

    [Header("State Control")]
    public bool canWalk;
    public bool canRun;
    public bool canCrouch;
    public bool blockPlayer;
    public List<IUsable> inventory = new List<IUsable>();
    public IUsable selectedObject;
    public Transform ObjectsTransform;

    [Header("Screech")]
    public float screechRadius;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 currentRotation;
    private Vector2 targetRotation;
    private Vector2 rotationVelocity;
    private float currentSpeed;
    private Vector3 velocity;
    private bool isPillEffectActive = false; // Nuevo booleano para controlar el efecto de la píldora
    private Coroutine pillEffectCoroutine; // Guarda la referencia de la corrutina de la píldora


    public enum PlayerState
    {
        Waiting,
        Walking,
        Running,
        Crouching,
        Hiding,
        Block,
        Dead
    }
    public PlayerState currentState;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentState = PlayerState.Walking;
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
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
        playerInput.actions["SelectObj3"].started += ctx => SelectObj(3);

        playerInput.actions["ActivateObj"].started += ctx => ActivateObj();
    }

    private void Update()
    {
        BlockPlayer();
        Move();
        Look();
        RotatePlayer();
        ApplyGravity();
    }

    // Movement-related methods
    private void Move()
    {
        if (currentState == PlayerState.Dead || currentState == PlayerState.Block || currentState == PlayerState.Hiding)
        {
            currentSpeed = 0f;
            return;
        }

        if (moveInput == Vector2.zero && currentState != PlayerState.Crouching)
        {
            ChangePlayerState(PlayerState.Waiting);
            currentSpeed = 0f;
            return;
        }
        else if (currentState == PlayerState.Waiting)
        {
            ChangePlayerState(PlayerState.Walking);
        }

        // Si el efecto de la píldora está activo, corre automáticamente (excepto si está agachado)
        if (isPillEffectActive && currentState != PlayerState.Crouching)
        {
            ChangePlayerState(PlayerState.Running);
        }

        float targetSpeed = currentState == PlayerState.Running ? runSpeed : (currentState == PlayerState.Crouching ? crouchSpeed : walkSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
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
        if (controller.isGrounded && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Player State Management
    private void StartRunning()
    {
        if (canRun && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            ChangePlayerState(PlayerState.Running);
        }
    }

    private void StopRunning()
    {
        if (canRun && currentState != PlayerState.Crouching && currentState != PlayerState.Block)
        {
            ChangePlayerState(PlayerState.Walking);
        }
    }

    public void ActivatePillEffect(float duration)
    {
        // Si la corrutina ya está activa, la detenemos para reiniciar el temporizador
        if (pillEffectCoroutine != null)
        {
            StopCoroutine(pillEffectCoroutine);
        }

        pillEffectCoroutine = StartCoroutine(PillEffectCoroutine(duration));
    }

    // Corrutina para el efecto de la píldora
    private IEnumerator PillEffectCoroutine(float duration)
    {
        isPillEffectActive = true;

        yield return new WaitForSeconds(duration);

        isPillEffectActive = false;
        ChangePlayerState(PlayerState.Walking); // Vuelve al estado Walking una vez termina el efecto
        pillEffectCoroutine = null; // Resetea la referencia de la corrutina
    }

    private void ToggleCrouch()
    {
        if (canCrouch && currentState != PlayerState.Block && currentState != PlayerState.Hiding)
        {
            if (currentState == PlayerState.Crouching)
            {
                controller.height = normalHeight;
                ChangePlayerState(PlayerState.Walking);
            }
            else
            {
                controller.height = crouchHeight;
                ChangePlayerState(PlayerState.Crouching);
            }
        }
    }

    private void BlockPlayer()
    {
        if (blockPlayer)
        {
            ChangePlayerState(PlayerState.Block);
        }
        else if (currentState == PlayerState.Block)
        {
            ChangePlayerState(PlayerState.Walking);
        }
    }

    public void EnterHiding(Vector3 insidePosition)
    {
        ChangePlayerState(PlayerState.Hiding);
        controller.height = normalHeight;
        StartCoroutine(MoveToPosition(insidePosition));
    }

    public void ExitHiding(Vector3 outsidePosition)
    {
        ChangePlayerState(PlayerState.Walking);
        StartCoroutine(MoveToPosition(outsidePosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float timeToMove = 1.0f;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    // Interaction methods
    private void Interact()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IUsable usable = hit.collider.GetComponent<IUsable>();
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.InteractObj();
            }

            if (usable != null && CheckPickUpItem(usable))
            {
                PickUpItem(usable);
                usable.GetObjPlayer(ObjectsTransform);
            }
        }
    }

    // Inventory and item selection methods
    private bool CheckPickUpItem(IUsable usableItem)
    {
        foreach (var item in inventory)
        {
            if (item.GetType() == usableItem.GetType())
            {
                //Debug.Log("Ya tienes un objeto de este tipo en el inventario.");
                return false;
            }
        }
        return true;
    }

    public void PickUpItem(IUsable usableItem)
    {
        if (inventory.Count < 6)
        {
            inventory.Add(usableItem);
            UpdateInventoryText();
        }
        else
        {
            //Debug.Log("Inventario lleno.");
        }
    }

    public void RemoveItem(IUsable usableItem)
    {
        if (inventory.Contains(usableItem))
        {
            selectedObject = null;
            inventory.Remove(usableItem);
            //Debug.Log($"{usableItem.GetType().ToString()} eliminado del inventario.");

            UpdateInventoryText();
        }
        else
        {
            //Debug.Log($"{usableItem.GetType().ToString()} no está en el inventario.");
        }
    }

    private void UpdateInventoryText()
    {
        Text_Objets.text = "Objetos en el inventario:";
        foreach (var item in inventory)
        {
            Text_Objets.text += $"\n {item.GetType().ToString()}";
        }
    }

    public void SelectObj(int index)
    {
        if (index > 0 && index <= inventory.Count)
        {
            var newSelectedObject = inventory[index - 1];

            if (selectedObject != newSelectedObject)
            {
                if (selectedObject != null)
                {
                    selectedObject.DesActivate();
                }

                selectedObject = newSelectedObject;
                selectedObject.Activate();
            }
            else
            {
                selectedObject.DesActivate();
                selectedObject = null;
            }
        }
        else
        {
            //Debug.Log("Invalid selection.");
        }
    }


    public void ActivateObj()
    {
        if (selectedObject != null)
        {
            selectedObject.Use();
        }
        else
        {
            //Debug.Log("No object selected.");
        }
    }

    // Health management methods
    public void TakeDamage(float amount)
    {
        if (currentState == PlayerState.Dead) return;
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (currentState == PlayerState.Dead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void Die()
    {
        currentState = PlayerState.Dead;
        currentHealth = 0;
        //Debug.Log("Player has died.");
    }

    public void KillInstantly() => Die();
    public float GetHealth() => currentHealth;

    // State transition and camera handling
    private void ChangePlayerState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnPlayerStateChanged(currentState.ToString());
        }
    }

    private void OnPlayerStateChanged(string stateName) => Text_State.text = stateName;

    private void Look()
    {
        targetRotation.x += lookInput.x * mouseSensitivity;
        targetRotation.y -= lookInput.y * mouseSensitivity;
        targetRotation.y = Mathf.Clamp(targetRotation.y, minVerticalAngle, maxVerticalAngle);

        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, rotationSmoothness);

        transform.rotation = Quaternion.Euler(0f, currentRotation.x, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(currentRotation.y, 0f, 0f);
    }

    private void RotatePlayer()
    {
        Quaternion cameraRotation = playerCamera.transform.rotation;
        transform.rotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, screechRadius);
        
    }
}
