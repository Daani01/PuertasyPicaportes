using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackController : Enemie
{
    public GameObject JackCollider;

    private void OnEnable()
    {
        damage = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Damage.ToString()));
        speed = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Speed.ToString()));
        string[] dieInfoArray = CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.DieInfo.ToString()).Split(';');
        dieInfo = dieInfoArray[Random.Range(0, dieInfoArray.Length)];

        float probability = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Probability.ToString()));
        if (Random.Range(0f, 100f) <= probability)
        {
            JackCollider.SetActive(true);
        }
        else
        {
            JackCollider.SetActive(false);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                if (player.currentState == FirstPersonController.PlayerState.Dead)
                {
                    return;
                }

                if (player.currentState != FirstPersonController.PlayerState.Dead && player.currentState == FirstPersonController.PlayerState.Hiding)
                {
                    player.TakeDamage(damage, gameObject.GetComponent<Enemie>());
                }
            }
        }
    }

}
