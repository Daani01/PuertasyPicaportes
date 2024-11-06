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
                player.currentHealth = Mathf.Min(player.currentHealth + HealAmount, player.maxHealth);
                gameObject.SetActive(false);
                Debug.Log($"Player healed by {HealAmount}. Current health: {player.currentHealth}");
            }
            else
            {
                Debug.Log("Player health is full or player is dead.");
            }
        }
    }
}
