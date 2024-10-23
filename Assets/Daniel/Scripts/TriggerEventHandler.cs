using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{
    private RoomEventManager roomEventManager;

    // Esta funci�n se ejecuta cuando algo entra en el TriggerEvent
    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si lo que entra en el trigger es el jugador (suponiendo que tenga el tag "Player")
        if (other.CompareTag("Player"))
        {
            // Llamar a la funci�n de RoomEventManager para activar el evento asignado
            roomEventManager = GetComponentInParent<RoomEventManager>();
            if (roomEventManager != null)
            {
                roomEventManager.OnPlayerEnterRoom(gameObject);
            }
            else
            {
                Debug.LogError("RoomEventManager no encontrado en la habitaci�n.");
            }
        }
    }
}
