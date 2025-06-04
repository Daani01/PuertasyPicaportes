using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RoomEventManager : MonoBehaviour
{
    public enum RoomEventType
    {
        None,
        Rush,
        Screech,
        Eyes,
        Ambush,
        A_60,
        A_90,
        A_120,
        End
    }

    private RoomEventType _eventType;
    private Dictionary<RoomEventType, int> eventProbabilities = new Dictionary<RoomEventType, int>();

    private void Awake()
    {
        LoadProbabilitiesFromCSV();
    }

    private void LoadProbabilitiesFromCSV()
    {
        foreach (RoomEventType eventType in Enum.GetValues(typeof(RoomEventType)))
        {
            if (eventType == RoomEventType.End)
                continue;

            string probabilityStr = CSVManager.Instance.GetSpecificData(eventType.ToString(), "Probability");

            if (int.TryParse(probabilityStr, out int probability))
            {
                eventProbabilities[eventType] = probability;
            }
            else
            {
                eventProbabilities[eventType] = 0;
            }
        }
    }



    public void AssignRoomEvent(GameObject room, int numberRoom, int maxNumberRoom)
    {
        int eventChance = Mathf.Clamp((numberRoom * 100) / maxNumberRoom, 0, 100);
        
        if (UnityEngine.Random.Range(0, 100) < eventChance)
        {
            _eventType = GetRandomEventByProbability();
        }
        else
        {
            _eventType = RoomEventType.None;
        }
        

        if (numberRoom > 0)
        {
            Transform triggerEvent = room.transform.Find("TriggerEvent");
            if (triggerEvent != null)
            {
                room.GetComponent<RoomEventManager>()._eventType = _eventType;

                if(_eventType != RoomEventType.None)
                {
                    Debug.Log($"Enemigo: {room.GetComponent<RoomEventManager>()._eventType} - Habitacion: {numberRoom}");
                }

            }
        }
    }

    private RoomEventType GetRandomEventByProbability()
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);

        foreach (var kvp in eventProbabilities.OrderByDescending(x => x.Value))
        {
            if (randomPoint < kvp.Value)
            {
                return kvp.Key;
            }
        }
        return eventProbabilities.Keys.ElementAt(UnityEngine.Random.Range(0, eventProbabilities.Count));
    }

    public void AssignEndRoomEvent(GameObject room)
    {
        Transform triggerEvent = room.transform.Find("TriggerEvent");
        if (triggerEvent != null)
        {
            room.GetComponent<RoomEventManager>()._eventType = RoomEventType.End;
        }
    }

    public void OnPlayerEnterRoom()
    {
        EnemyManager.Instance?.HandlePlayerEnter(_eventType);
    }
}
