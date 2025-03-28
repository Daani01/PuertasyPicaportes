using System.Collections.Generic;
using UnityEngine;
using static RoomEventManager;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject pf_Rush;
    public GameObject pf_Ambush;
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

        //Debug.Log($"Enemigo: {eventType} activado");


        switch (eventType)
        {
            case RoomEventType.None:
                break;
            case RoomEventType.Rush:
                List<Transform> rushTransforms = roomGenerator.GetTransforms(4);
                if (rushTransforms.Count > 0)
                {
                    Transform initialSpawnPoint = rushTransforms[0];

                    GameObject rushInstance = EnemyPool.Instance.GetEnemy(pf_Rush, initialSpawnPoint.position, initialSpawnPoint.rotation);
                    RushController rushController = rushInstance.GetComponent<RushController>();
                    rushController.SetWaypoints(rushTransforms);

                }
                GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().StartAlertEnemy();

                break;
            case RoomEventType.Ambush:
                List<Transform> ambushTransforms = roomGenerator.GetTransforms(10);
                if (ambushTransforms.Count > 0)
                {
                    Transform initialSpawnPoint = ambushTransforms[0];

                    GameObject ambushInstance = EnemyPool.Instance.GetEnemy(pf_Ambush, initialSpawnPoint.position, initialSpawnPoint.rotation);
                    AmbushController ambushController = ambushInstance.GetComponent<AmbushController>();
                    ambushController.SetWaypoints(ambushTransforms);

                }
                GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().StartAlertEnemy();

                break;
            case RoomEventType.Screech:
                GameObject screechInstance = EnemyPool.Instance.GetEnemy(pf_Screech, Vector3.zero, Quaternion.identity);
                GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().StartAlertEnemy();

                break;
            case RoomEventType.Eyes:
                GameObject eyesInstance = EnemyPool.Instance.GetEnemy(pf_Eyes, Vector3.zero, Quaternion.identity);
                EyesController eyesController = eyesInstance.GetComponent<EyesController>();
                eyesController.SetPosition(roomGenerator.GetTransformEyes());
                GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().StartAlertEnemy();

                break;            
            case RoomEventType.End:
                GameObject.Find("GAMELOOP").GetComponent<GameLoop>().PlayerEndGameWin();
                break;
        
        }
    }


}
