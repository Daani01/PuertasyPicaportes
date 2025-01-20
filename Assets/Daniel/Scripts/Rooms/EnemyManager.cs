using System.Collections.Generic;
using UnityEngine;
using static RoomEventManager;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject pf_Rush;
    public GameObject pf_Screech;

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

                List<Transform> lastRoomTransforms = roomGenerator.GetTransformsRush(4);
                if (lastRoomTransforms.Count > 0)
                {
                    Transform initialSpawnPoint = lastRoomTransforms[0];

                    GameObject rushInstance = Instantiate(pf_Rush, initialSpawnPoint.position, initialSpawnPoint.rotation);

                    RushController rushController = rushInstance.GetComponent<RushController>();
                    if (rushController != null)
                    {
                        rushController.SetWaypoints(lastRoomTransforms);
                    }
                }
                break;
            case RoomEventType.Screech:

                GameObject screechInstance = Instantiate(pf_Screech);

                break;

        }
    }

}
