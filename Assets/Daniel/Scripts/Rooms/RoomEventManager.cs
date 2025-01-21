using UnityEngine;

public class RoomEventManager : MonoBehaviour
{
    public enum RoomEventType
    {
        None,
        Rush,
        Ambush,
        Halt,
        Eyes,
        Screech,
        Dupe,
        A_60,
        A_90,
        A_120
    }

    private RoomEventType _eventType;
    
    public void AssignRoomEvent(GameObject room, int numberRoom, int maxNumberRoom)
    {
        //int random = Random.Range(0, maxNumberRoom);

        if (numberRoom > 0)
        {
            Transform triggerEvent = room.transform.Find("TriggerEvent");
            if (triggerEvent != null)
            {
                _eventType = RoomEventType.Screech;
                room.GetComponent<RoomEventManager>()._eventType = _eventType;
                Debug.Log($"Asignando evento {_eventType} en la habitación {numberRoom}");
            }
        }
        else
        {

            //Transform triggerEvent = room.transform.Find("TriggerEvent");
            //triggerEvent.gameObject.SetActive(false);
            //Debug.Log($"No se generó evento para la habitación {numberRoom}");

        }
    }

    public void OnPlayerEnterRoom()
    {
        EnemyManager.Instance?.HandlePlayerEnter(_eventType);
    }
}
