using UnityEngine;
using static FirstPersonController;

public class Wardrobe : MonoBehaviour, Interactable
{
    Transform insideWardrobe;
    Transform outsideWardrobe;

    private void OnEnable()
    {
        insideWardrobe = transform.Find("INSIDE_WARDROBE");
        outsideWardrobe = transform.Find("OUTSIDE_WARDROBE");

    }

    public void Interact()
    {

        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            if (player.currentState == PlayerState.Hiding)
            {
                player.ExitHiding(outsideWardrobe.position); // Mover a la posición de vuelta
            }
            else
            {
                player.EnterHiding(insideWardrobe.position); // Mover a la posición dentro del armario
            }
        }
    }
}
