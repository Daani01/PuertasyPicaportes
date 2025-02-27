using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FirstPersonController;

public class Bandage : MonoBehaviour, IInteractable
{
    public int HealAmount;

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            if (player.currentHealth > 0 && player.currentHealth < player.maxHealth)
            {
                player.Heal(HealAmount);
                gameObject.SetActive(false);
            }
            else
            {
                player.ShowMessage("Vida completa", 4.0f);
            }
        }
    }
}
