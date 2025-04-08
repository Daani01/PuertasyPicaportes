using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawerObjectManager : MonoBehaviour
{
    private enum ObjectType
    {
        Flashlight,
        ChargeFlashlight,
        Lighter,
        Coins,
        Bandage,
        Pills,
        Crucifix
    }

    public GameObject[] objectsPrefabs;
    public Transform[] spawnPoints;

    private Dictionary<ObjectType, int> objectProbabilities = new Dictionary<ObjectType, int>();

    private void Awake()
    {
        LoadProbabilitiesFromCSV();
        GenerateObjects();
    }

    private void LoadProbabilitiesFromCSV()
    {
        foreach (ObjectType objType in Enum.GetValues(typeof(ObjectType)))
        {
            string probabilityStr = CSVManager.Instance.GetSpecificData(objType.ToString(), "Probability");

            if (int.TryParse(probabilityStr, out int probability))
            {
                objectProbabilities[objType] = probability;
            }
            else
            {
                objectProbabilities[objType] = 0; // Asigna 0 si no encuentra la probabilidad
            }
        }
    }

    void GenerateObjects()
    {
        ObjectType selectedObject = GetRandomObjectByProbability();

        int prefabIndex = (int)selectedObject; // Ya no hay None, así que no restamos 1
        if (prefabIndex >= 0 && prefabIndex < objectsPrefabs.Length)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject newObj = Instantiate(objectsPrefabs[prefabIndex], spawnPoint);
            //Debug.Log($"Objeto generado: {objectsPrefabs[prefabIndex].name}");
        }
    }

    private ObjectType GetRandomObjectByProbability()
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);

        foreach (var kvp in objectProbabilities.OrderByDescending(x => x.Value))
        {
            if (randomPoint < kvp.Value)
            {
                return kvp.Key;
            }
        }

        return objectProbabilities.Keys.ElementAt(UnityEngine.Random.Range(0, objectProbabilities.Count));
    }
}
