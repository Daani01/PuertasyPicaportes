using UnityEngine;
using static FirstPersonController;

public class Coins : MonoBehaviour, IInteractable
{
    public int value;

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.coinsCount += value;
            gameObject.SetActive(false);
        }
    }

}
