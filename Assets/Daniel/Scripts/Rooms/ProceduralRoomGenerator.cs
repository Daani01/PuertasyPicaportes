using UnityEngine;
using System.Collections.Generic;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject finishRoomPrefab;
    public GameObject[] basicRoomPrefabs;
    public GameObject doorPrefab;

    public int numberOfRooms;
    public int roomCount;
    public int maxRoomActived;

    public List<GameObject> rooms = new List<GameObject>();

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        RoomEventManager roomEventManager = GetComponent<RoomEventManager>();

        GameObject startRoom = Instantiate(startRoomPrefab);
        rooms.Add(startRoom);

        Transform previousEndDoor = startRoom.transform.Find("EndDoorSpawnPoint");

        for (int i = 0; i < numberOfRooms; i++)
        {
            // Crear una nueva sala
            GameObject newRoomPrefab = basicRoomPrefabs[Random.Range(0, basicRoomPrefabs.Length)];
            GameObject newRoom = Instantiate(newRoomPrefab);
            roomEventManager.AssignRoomEvent(newRoom, i, numberOfRooms);

            Transform newStartDoor = newRoom.transform.Find("StartDoorSpawnPoint");

            AlignRooms(previousEndDoor, newStartDoor);

            //CreateDoor(previousEndDoor);//CAMBIAR EL TRANSFORM

            rooms.Add(newRoom);

            previousEndDoor = newRoom.transform.Find("EndDoorSpawnPoint");
        }

        GameObject finishRoom = Instantiate(finishRoomPrefab);
        Transform finishStartDoor = finishRoom.transform.Find("StartDoorSpawnPoint");

        AlignRooms(previousEndDoor, finishStartDoor);
        rooms.Add(finishRoom);


        // Activar solo las primeras 3 habitaciones y desactivar el resto
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i < maxRoomActived)
            {
                rooms[i].SetActive(true); // Activar las primeras 3 habitaciones
            }
            else
            {
                rooms[i].SetActive(false); // Desactivar las restantes
            }
        }
    }

    void CreateDoor(Transform doorSpawnPoint)
    {
        GameObject door = Instantiate(doorPrefab, doorSpawnPoint.position, doorSpawnPoint.rotation);
        door.transform.SetParent(doorSpawnPoint);
    }

    void AlignRooms(Transform previousEnd, Transform newStart)
    {
        Vector3 positionOffset = newStart.position - newStart.parent.position;
        newStart.parent.position = previousEnd.position - positionOffset;
        newStart.parent.rotation = previousEnd.rotation;
    }

    public void IncreaseRoomCount()
    {
        // Aumentar el contador de habitaciones
        roomCount++;

        // Asegurarse de que roomCount no exceda el número total de habitaciones
        if (roomCount > numberOfRooms)
        {
            //roomCount = numberOfRooms - 1;
            return; // No realizar más acciones si no hay más habitaciones para activar
        }

        // Activar la nueva habitación dentro del rango
        if (roomCount < rooms.Count && (roomCount + maxRoomActived - 1) < rooms.Count)
        {
            rooms[roomCount + (maxRoomActived - 1)].SetActive(true);
        }

        // Desactivar la habitación que está fuera del rango si roomCount es mayor o igual a 3
        if (roomCount > 2)
        {
            int roomToDeactivate = roomCount - 3;
            if (roomToDeactivate >= 0 && roomToDeactivate < rooms.Count)
            {
                rooms[roomToDeactivate].SetActive(false);
            }
        }
    }


    public void DecreaseRoomCount()
    {
        roomCount--;
    }

    public int GetCurrentRoomIndex()
    {
        return roomCount;
    }

    public Transform GetCurrentRoomTransform()
    {
        if (roomCount >= 0 && roomCount < rooms.Count)
        {
            return rooms[roomCount].transform;
        }
        return null;
    }

    public List<Transform> GetTransformsRush(int count)
    {
        List<Transform> lastRoomTransforms = new List<Transform>();

        int startIndex = Mathf.Max(0, GetCurrentRoomIndex() - count);

        for (int i = startIndex; i <= GetCurrentRoomIndex() + count; i++)
        {
            if(i < numberOfRooms)
            {
                lastRoomTransforms.Add(rooms[i].transform.Find("StartDoorSpawnPoint"));
                lastRoomTransforms.Add(rooms[i].transform.Find("EndDoorSpawnPoint"));
            }            
        }

        return lastRoomTransforms;
    }


}
