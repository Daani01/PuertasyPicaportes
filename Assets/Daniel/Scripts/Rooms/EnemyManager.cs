using System.Collections.Generic;
using UnityEngine;
using static RoomEventManager;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject pf_Rush;
    public GameObject pf_Screech;
    public GameObject pf_Eyes;

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

        Debug.Log($"Enemigo: {eventType} activado");


        switch (eventType)
        {
            case RoomEventType.None:
                break;
            case RoomEventType.Rush:
                List<Transform> lastRoomTransforms = roomGenerator.GetTransformsRush(4);
                if (lastRoomTransforms.Count > 0)
                {
                    Transform initialSpawnPoint = lastRoomTransforms[0];

                    GameObject rushInstance = EnemyPool.Instance.GetEnemy(pf_Rush, initialSpawnPoint.position, initialSpawnPoint.rotation);
                    RushController rushController = rushInstance.GetComponent<RushController>();
                    rushController.SetWaypoints(lastRoomTransforms);

                }
                break;
            case RoomEventType.Screech:
                GameObject screechInstance = EnemyPool.Instance.GetEnemy(pf_Screech, Vector3.zero, Quaternion.identity);
                break;
            case RoomEventType.Eyes:
                GameObject eyesInstance = EnemyPool.Instance.GetEnemy(pf_Eyes, Vector3.zero, Quaternion.identity);
                EyesController eyesController = eyesInstance.GetComponent<EyesController>();
                eyesController.SetPosition(roomGenerator.GetTransformEyes());
                break;
        
        }
    }


}
