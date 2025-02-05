using UnityEngine;

public class RoomEventManager : MonoBehaviour
{
    public enum RoomEventType
    {
        None,
        Rush,
        Screech,
        Eyes,
        Ambush,
        Halt,
        Dupe,
        A_60,
        A_90,
        A_120
    }

    private RoomEventType _eventType;

    public void AssignRoomEvent(GameObject room, int numberRoom, int maxNumberRoom)
    {
        // Selecciona aleatoriamente uno de los primeros 4 tipos de RoomEventType
        int randomFirst4 = Random.Range(0, 4); // Rush, Screech, Eyes, Ambush
        _eventType = (RoomEventType)randomFirst4;

        // Descomentar para elegir aleatoriamente de todos los tipos de RoomEventType
        /*
        int randomAll = Random.Range(0, System.Enum.GetValues(typeof(RoomEventType)).Length);
        _eventType = (RoomEventType)randomAll;
        */

        if (numberRoom > 0)
        {
            Transform triggerEvent = room.transform.Find("TriggerEvent");
            if (triggerEvent != null)
            {
                room.GetComponent<RoomEventManager>()._eventType = _eventType;
                //Debug.Log($"Asignando evento {_eventType} en la habitación {numberRoom}");
            }
        }

    }

    public void OnPlayerEnterRoom()
    {
        EnemyManager.Instance?.HandlePlayerEnter(_eventType);
    }
}
