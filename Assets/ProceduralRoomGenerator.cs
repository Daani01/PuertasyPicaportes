using UnityEngine;
using System.Collections.Generic;

public class ProceduralRoomGenerator : MonoBehaviour
{
    // Prefabs de los distintos tipos de habitaciones
    public GameObject startRoomPrefab;
    public GameObject finishRoomPrefab;
    public GameObject[] basicRoomPrefabs;  // Prefabs de habitaciones intermedias (Basic, Large, Small, Corridor)

    public int numberOfRooms = 10;  // Número total de habitaciones intermedias

    private List<GameObject> rooms = new List<GameObject>(); // Lista de habitaciones generadas

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        // 1. Generar la primera habitación (Start) que solo tiene EndDoorSpawnPoint
        GameObject startRoom = Instantiate(startRoomPrefab);
        rooms.Add(startRoom);

        // Guardar el EndDoorSpawnPoint de la habitación inicial para alinear la siguiente
        Transform previousEndDoor = startRoom.transform.Find("EndDoorSpawnPoint");

        // 2. Generar las habitaciones intermedias
        for (int i = 0; i < numberOfRooms; i++)
        {
            // Seleccionar un prefab de habitación intermedia al azar
            GameObject newRoomPrefab = basicRoomPrefabs[Random.Range(0, basicRoomPrefabs.Length)];
            GameObject newRoom = Instantiate(newRoomPrefab);

            // Obtener el StartDoorSpawnPoint de la nueva habitación
            Transform newStartDoor = newRoom.transform.Find("StartDoorSpawnPoint");

            // Alinear la nueva habitación con el EndDoorSpawnPoint de la anterior
            AlignRooms(previousEndDoor, newStartDoor);

            // Añadir la nueva habitación a la lista
            rooms.Add(newRoom);

            // Actualizar el EndDoorSpawnPoint para la próxima habitación
            previousEndDoor = newRoom.transform.Find("EndDoorSpawnPoint");
        }

        // 3. Generar la última habitación (Finish) que solo tiene StartDoorSpawnPoint
        GameObject finishRoom = Instantiate(finishRoomPrefab);
        Transform finishStartDoor = finishRoom.transform.Find("StartDoorSpawnPoint");

        // Alinear la última habitación
        AlignRooms(previousEndDoor, finishStartDoor);
        rooms.Add(finishRoom);
    }

    // Método para alinear las habitaciones usando los puntos de spawn de las puertas
    void AlignRooms(Transform previousEnd, Transform newStart)
    {
        // Ajustar la posición y rotación de la nueva habitación para que coincida su StartDoorSpawnPoint con el EndDoorSpawnPoint anterior
        Vector3 positionOffset = newStart.position - newStart.parent.position;
        newStart.parent.position = previousEnd.position - positionOffset;
        newStart.parent.rotation = previousEnd.rotation;
    }
}
