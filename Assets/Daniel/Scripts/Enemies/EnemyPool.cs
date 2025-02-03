using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (var prefab in prefabEnemies)
        {
            Pool pool = new Pool { prefab = prefab };
            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemy = Instantiate(prefab);
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
        if (poolDictionary.ContainsKey(prefab) && poolDictionary[prefab].enemies.Count > 0)
        {
            GameObject enemy = poolDictionary[prefab].enemies.Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
            return enemy;
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
