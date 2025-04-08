using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemie;

public class EnemyPool : MonoBehaviour, IProcess
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public Queue<GameObject> enemies = new Queue<GameObject>();
    }

    public List<GameObject> prefabEnemies; // Lista de prefabs de enemigos
    [SerializeField] private int poolSize = 10;
    private Dictionary<GameObject, Pool> poolDictionary = new Dictionary<GameObject, Pool>();

    private Transform allRooms;

    public bool IsCompleted { get; private set; } = false;
    public static EnemyPool Instance { get; private set; }

    private Dictionary<GameObject, GameObject> enemyToPrefab = new Dictionary<GameObject, GameObject>(); // Mapa de enemigos a sus prefabs

    void Awake()
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

    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(InitializePool(onComplete));
    }

    private IEnumerator InitializePool(System.Action onComplete)
    {
        if (allRooms == null)
            allRooms = GameObject.Find("ALLROOMS").GetComponent<Transform>();

        foreach (var prefab in prefabEnemies)
        {
            Pool pool = new Pool { prefab = prefab };
            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemy = Instantiate(prefab);
                enemy.transform.SetParent(allRooms);

                enemy.GetComponent<Enemie>().damage = float.Parse(CSVManager.Instance.GetSpecificData(enemy.GetComponent<Enemie>().enemyName, ExcelValues.Damage.ToString()));
                enemy.GetComponent<Enemie>().speed = float.Parse(CSVManager.Instance.GetSpecificData(enemy.GetComponent<Enemie>().enemyName, ExcelValues.Speed.ToString()));
                string[] dieInfoArray = CSVManager.Instance.GetSpecificData(enemy.GetComponent<Enemie>().enemyName, ExcelValues.DieInfo.ToString()).Split(';');
                enemy.GetComponent<Enemie>().dieInfo = dieInfoArray[Random.Range(0, dieInfoArray.Length)];

                enemy.SetActive(false);

                enemyToPrefab[enemy] = prefab; // Guardamos la referencia al prefab original

                pool.enemies.Enqueue(enemy);
            }
            poolDictionary.Add(prefab, pool);
            yield return null;
        }

        IsCompleted = true;
        onComplete?.Invoke();
    }

    public GameObject GetEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(prefab))
        {
            Pool pool = poolDictionary[prefab];

            // Si solo queda 1 enemigo en la pool, generamos uno nuevo y lo agregamos a la pool
            if (pool.enemies.Count == 1)
            {
                GameObject newEnemy = Instantiate(prefab);
                newEnemy.GetComponent<Enemie>().damage = float.Parse(CSVManager.Instance.GetSpecificData(newEnemy.GetComponent<Enemie>().enemyName, ExcelValues.Damage.ToString()));
                newEnemy.GetComponent<Enemie>().speed = float.Parse(CSVManager.Instance.GetSpecificData(newEnemy.GetComponent<Enemie>().enemyName, ExcelValues.Speed.ToString()));
                string[] dieInfoArray = CSVManager.Instance.GetSpecificData(newEnemy.GetComponent<Enemie>().enemyName, ExcelValues.DieInfo.ToString()).Split(';');
                newEnemy.GetComponent<Enemie>().dieInfo = dieInfoArray[Random.Range(0, dieInfoArray.Length)];

                newEnemy.SetActive(false);
                enemyToPrefab[newEnemy] = prefab;
                pool.enemies.Enqueue(newEnemy);
            }

            if (pool.enemies.Count > 0)
            {
                GameObject enemy = pool.enemies.Dequeue();
                enemy.transform.position = position;
                enemy.transform.rotation = rotation;
                enemy.SetActive(true);
                return enemy;
            }
        }

        return null;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemyToPrefab.TryGetValue(enemy, out GameObject prefab)) // Obtenemos el prefab original
        {
            enemy.SetActive(false);
            poolDictionary[prefab].enemies.Enqueue(enemy);
        }
        else
        {
            Debug.LogError($"El enemigo {enemy.name} no tiene un prefab registrado en la pool.");
        }
    }
}
