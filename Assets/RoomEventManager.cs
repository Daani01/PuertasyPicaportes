using UnityEngine;

public class RoomEventManager : MonoBehaviour
{
    // Función principal que se llamará al crear la habitación y asignar el evento
    public void AssignRoomEvent(GameObject room)
    {
        // Buscar el objeto TriggerEvent dentro de la habitación
        Transform triggerEvent = room.transform.Find("TriggerEvent");
        if (triggerEvent != null)
        {
            Debug.Log("Asignando evento al TriggerEvent en " + room.name);
            DetermineRoomEvent(triggerEvent.gameObject);
        }
        else
        {
            Debug.LogError("No se encontró TriggerEvent en " + room.name);
        }
    }

    // Esta función determinará el tipo de evento que ocurrirá cuando el jugador entre en el TriggerEvent
    private void DetermineRoomEvent(GameObject triggerEvent)
    {
        Debug.Log("Evento determinado para TriggerEvent: " + triggerEvent.name);
        // Aquí decides el evento que ocurrirá (enemigos, trampas, etc.)
    }

    // Función que se llama cuando el jugador entra en la habitación
    public void OnPlayerEnterRoom(GameObject triggerEvent)
    {
        Debug.Log("Jugador entró en la habitación con TriggerEvent: " + triggerEvent.name);
        // Aquí puedes ejecutar el evento determinado para la habitación
    }
}
