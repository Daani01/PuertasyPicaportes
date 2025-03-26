using UnityEngine;
using static Enemie;
using static FirstPersonController;

public class Bandage : MonoBehaviour, IInteractable
{
    private int healAmount;

    private void Awake()
    {
        healAmount = int.Parse(CSVManager.Instance.GetSpecificData("Bandage", "Amount"));
    }

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            if (player.currentHealth > 0 && player.currentHealth < player.maxHealth)
            {
                player.Heal(healAmount);
                gameObject.SetActive(false);
            }
            else
            {
                player.ShowMessage("Vida completa", 4.0f);
            }
        }
    }
}
