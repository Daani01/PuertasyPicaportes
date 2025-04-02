using System.Collections.Generic;
using UnityEngine;
using static RoomEventManager;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject pf_Rush;
    public GameObject pf_Ambush;
    public GameObject pf_A_60;
    public GameObject pf_A_90;
    public GameObject pf_A_120;
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
                List<Transform> rushTransforms = roomGenerator.GetTransforms(6);
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
            case RoomEventType.A_60:
                List<Transform> a_60Transforms = roomGenerator.GetTransforms(15);
                if (a_60Transforms.Count > 0)
                {
                    Transform initialSpawnPoint = a_60Transforms[0];
                    GameObject a_60Instance = EnemyPool.Instance.GetEnemy(pf_A_60, initialSpawnPoint.position, initialSpawnPoint.rotation);
                    A60Controller a_60Controller = a_60Instance.GetComponent<A60Controller>();
                    a_60Controller.SetWaypoints(a_60Transforms);
                }
                break;
            case RoomEventType.A_90:
                GameObject A90Instance = EnemyPool.Instance.GetEnemy(pf_A_90, Vector3.zero, Quaternion.identity);
                break;
            case RoomEventType.A_120:
                List<Transform> a_120Transforms = roomGenerator.GetReverseTransforms(8);
                if (a_120Transforms.Count > 0)
                {
                    Transform initialSpawnPoint = a_120Transforms[0];
                    GameObject a_120Instance = EnemyPool.Instance.GetEnemy(pf_A_120, initialSpawnPoint.position, initialSpawnPoint.rotation);
                    A120Controller a_120Controller = a_120Instance.GetComponent<A120Controller>();
                    a_120Controller.SetWaypoints(a_120Transforms);
                }
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
