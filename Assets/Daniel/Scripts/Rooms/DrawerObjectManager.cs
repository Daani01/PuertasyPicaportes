using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerObjectManager : MonoBehaviour
{
    private enum ObjectType
    {
        None,
        Flashlight,
        Lighter,
        Coin,
        Coins,
        Bandage,
        Spider,
        Pills
    }

    public GameObject[] objectsPrefabs;
    public Transform[] spawnPoints;


    void Start()
    {
        GenerateObjects();
    }

    void GenerateObjects()
    {
        GameObject newObj = Instantiate(objectsPrefabs[Random.Range(0, objectsPrefabs.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)]);
    }
}
