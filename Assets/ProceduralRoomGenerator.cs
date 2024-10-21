using UnityEngine;
using System.Collections.Generic;

public class ProceduralRoomGenerator : MonoBehaviour
{
    // Prefabs de los distintos tipos de habitaciones
    public GameObject startRoomPrefab;
    public GameObject finishRoomPrefab;
    public GameObject[] basicRoomPrefabs;  // Prefabs de habitaciones intermedias (Basic, Large, Small, Corridor)

    public int numberOfRooms = 10;  // N�mero total de habitaciones intermedias

    private List<GameObject> rooms = new List<GameObject>(); // Lista de habitaciones generadas

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        // 1. Generar la primera habitaci�n (Start) que solo tiene EndDoorSpawnPoint
        GameObject startRoom = Instantiate(startRoomPrefab);
        rooms.Add(startRoom);

        // Guardar el EndDoorSpawnPoint de la habitaci�n inicial para alinear la siguiente
        Transform previousEndDoor = startRoom.transform.Find("EndDoorSpawnPoint");

        // 2. Generar las habitaciones intermedias
        for (int i = 0; i < numberOfRooms; i++)
        {
            // Seleccionar un prefab de habitaci�n intermedia al azar
            GameObject newRoomPrefab = basicRoomPrefabs[Random.Range(0, basicRoomPrefabs.Length)];
            GameObject newRoom = Instantiate(newRoomPrefab);

            // Obtener el StartDoorSpawnPoint de la nueva habitaci�n
            Transform newStartDoor = newRoom.transform.Find("StartDoorSpawnPoint");

            // Alinear la nueva habitaci�n con el EndDoorSpawnPoint de la anterior
            AlignRooms(previousEndDoor, newStartDoor);

            // A�adir la nueva habitaci�n a la lista
            rooms.Add(newRoom);

            // Actualizar el EndDoorSpawnPoint para la pr�xima habitaci�n
            previousEndDoor = newRoom.transform.Find("EndDoorSpawnPoint");
        }

        // 3. Generar la �ltima habitaci�n (Finish) que solo tiene StartDoorSpawnPoint
        GameObject finishRoom = Instantiate(finishRoomPrefab);
        Transform finishStartDoor = finishRoom.transform.Find("StartDoorSpawnPoint");

        // Alinear la �ltima habitaci�n
        AlignRooms(previousEndDoor, finishStartDoor);
        rooms.Add(finishRoom);
    }

    // M�todo para alinear las habitaciones usando los puntos de spawn de las puertas
    void AlignRooms(Transform previousEnd, Transform newStart)
    {
        // Ajustar la posici�n y rotaci�n de la nueva habitaci�n para que coincida su StartDoorSpawnPoint con el EndDoorSpawnPoint anterior
        Vector3 positionOffset = newStart.position - newStart.parent.position;
        newStart.parent.position = previousEnd.position - positionOffset;
        newStart.parent.rotation = previousEnd.rotation;
    }
}
