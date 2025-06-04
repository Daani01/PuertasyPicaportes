using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemie : MonoBehaviour
{
    public string enemyName;
    public float damage;
    public float speed;
    public string dieInfo;
    public enum ExcelValues
    {
        Damage,
        DieInfo,
        Probability,
        Speed
    }
}
