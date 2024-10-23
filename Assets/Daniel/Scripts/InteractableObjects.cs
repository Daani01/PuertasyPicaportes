using UnityEngine;

public interface Interactable
{
    void Interact();
}

public interface IUsable
{
    void Use();
}

public enum UsableType
{
    None,
    Flashlight,
    Coin,
    Key
}

public enum ActivationType
{
    OneTime,          // Se activa solo una vez
    OneTimeToggle,    // Se activa una vez, luego se desactiva
    MultipleTimes     // Se puede activar/desactivar varias veces
}

public class InteractableObjects : MonoBehaviour, Interactable, IUsable
{
    public UsableType usableType;
    public bool canActivateMultipleTimes = false; // Si se puede usar varias veces

    public ActivationType activationType;
    private bool hasBeenUsed = false;

    private bool isActive = false; // Estado del objeto si est� activo o no

    public void Interact()
    {
        Debug.Log($"Interacted with {gameObject.name}");
        // Aqu� puedes agregar cualquier l�gica adicional para la interacci�n

        if (usableType != UsableType.None)
        {
            PickUpItem();
        }
    }

    public void PickUpItem()
    {
        // Si el objeto es activable solo una vez y ya est� activo, no hacer nada
        if (!canActivateMultipleTimes && isActive)
        {
            Debug.Log($"{gameObject.name} ya est� activo.");
            return;
        }

        // L�gica para recoger el objeto
        isActive = !isActive; // Cambia el estado activo/inactivo

        if (isActive)
        {
            Debug.Log($"{gameObject.name} ha sido recogido.");
            gameObject.SetActive(false);
            // Aqu� puedes agregar la l�gica para activar el objeto
        }
        else
        {
            Debug.Log($"{gameObject.name} ha sido soltado.");
            // Aqu� puedes agregar la l�gica para desactivar el objeto
        }
    }

    public void Use()
    {
        switch (activationType)
        {
            case ActivationType.OneTime:
                if (!hasBeenUsed)
                {
                    Activate();
                    hasBeenUsed = true;
                }
                break;

            case ActivationType.OneTimeToggle:
                if (!hasBeenUsed)
                {
                    Activate();
                    hasBeenUsed = true;
                }
                else
                {
                    Deactivate();
                }
                break;

            case ActivationType.MultipleTimes:
                if (isActive)
                {
                    Deactivate();
                }
                else
                {
                    Activate();
                }
                break;
        }
    }

    private void Activate()
    {
        isActive = true;
        Debug.Log($"{gameObject.name} Activated.");
    }

    private void Deactivate()
    {
        isActive = false;
        Debug.Log($"{gameObject.name} Deactivated.");
    }
}
