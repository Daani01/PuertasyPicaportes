using UnityEngine;
using UnityEngine.Playables;
using static FirstPersonController;

public class Wardrobe : MonoBehaviour, Interactable
{
    public void Interact()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            if (player.currentState == PlayerState.Hiding)
            {
                player.ExitHiding(); //A�ADIR POSICION DE VUELTA
            }
            else
            {
                player.EnterHiding(); //A�ADIR POSICION DE IDA
            }
        }
    }

}
