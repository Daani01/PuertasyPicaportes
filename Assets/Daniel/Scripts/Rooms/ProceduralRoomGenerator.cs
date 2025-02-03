using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ProceduralRoomGenerator : MonoBehaviour, IProcess
{

    public bool IsCompleted { get; private set; } = false;

    public GameObject startRoomPrefab;
    public GameObject finishRoomPrefab;
    public GameObject[] basicRoomPrefabs;
    public GameObject doorPrefab;

    public int numberOfRooms;
    public int roomCount;
    public int maxRoomActived;

    public List<GameObject> rooms = new List<GameObject>();
    private RoomEventManager roomEventManager;


    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(ProcessRoutine(onComplete));
    }

    private IEnumerator ProcessRoutine(System.Action onComplete)
    {

        yield return StartCoroutine(GenerateRooms());

        IsCompleted = true;

        onComplete?.Invoke();
    }


    private IEnumerator GenerateRooms()
    {
        roomEventManager = GetComponent<RoomEventManager>();
        if (roomEventManager == null)
        {
            Debug.LogError("No se encontró RoomEventManager en el objeto.");
            yield break;
        }

        GameObject startRoom = Instantiate(startRoomPrefab);
        rooms.Add(startRoom);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Obtener la posición de la primera habitación (StartDoorSpawnPoint)
            Transform playerSpawnPoint = startRoom.transform.Find("PlayerSpawnPoint");
            if (playerSpawnPoint != null)
            {
                // Colocar al jugador en la posición de la primera habitación
                player.transform.position = playerSpawnPoint.position;
            }
            else
            {
                Debug.LogError("No se encontró StartDoorSpawnPoint en la primera habitación.");
            }
        }
        else
        {
            Debug.LogError("No se encontró al jugador en la escena.");
        }


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


        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].SetActive(i < maxRoomActived);
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
        if (roomCount > maxRoomActived)
        {
            int roomToDeactivate = roomCount - (maxRoomActived + 1);
            if (roomToDeactivate >= 0 && roomToDeactivate + 1 < rooms.Count)
            {
                GameObject room = rooms[roomToDeactivate + 1];
                Transform doorTransform = room.transform.Find("pf_Door");

                if (doorTransform != null)
                {
                    Doors door = doorTransform.GetComponent<Doors>();
                    if (door != null)
                    {
                        door.StartCoroutine(door.CloseDoor());
                    }
                }

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

    public Transform GetTransformEyes()
    {
        return rooms[GetCurrentRoomIndex()].transform.Find("EyesSpawnPoint");
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
