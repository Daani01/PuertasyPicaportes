using System.Collections.Generic;
using UnityEngine;
using static RoomEventManager;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject pf_Rush;
    public ProceduralRoomGenerator roomGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void HandlePlayerEnter(RoomEventType eventType)
    {
        roomGenerator.IncreaseRoomCount();
        switch (eventType)
        {
            case RoomEventType.None:
                break;
            case RoomEventType.Rush:

                List<Transform> lastRoomTransforms = roomGenerator.GetLastRoomTransforms(3);
                if (lastRoomTransforms.Count > 0)
                {
                    Transform initialSpawnPoint = lastRoomTransforms[0].Find("StartDoorSpawnPoint");

                    // Instancia el prefab en la posición inicial
                    GameObject rushInstance = Instantiate(pf_Rush, initialSpawnPoint.position, initialSpawnPoint.rotation);

                    // Obtiene el componente RushController desde la instancia de rushInstance y pasa los waypoints
                    RushController rushController = rushInstance.GetComponent<RushController>();
                    if (rushController != null)
                    {
                        rushController.SetWaypoints(lastRoomTransforms);
                    }
                }
                break;

        }
    }

}
