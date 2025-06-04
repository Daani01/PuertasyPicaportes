using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBandage : MonoBehaviour, IInteractable
{
    private int healAmount;

    private void Awake()
    {
        healAmount = int.Parse(CSVManager.Instance.GetSpecificData("GoldBandage", "Amount"));
    }

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.ExtraHeal(healAmount);
            gameObject.SetActive(false);
        }
    }
}
