using UnityEngine;
using static FirstPersonController;

public class Wardrobe : MonoBehaviour, IInteractable
{
    Transform insideWardrobe;
    Transform outsideWardrobe;

    private void OnEnable()
    {
        Transform parent = transform.parent;

        if (parent != null)
        {
            insideWardrobe = parent.Find("INSIDE_WARDROBE");
            outsideWardrobe = parent.Find("OUTSIDE_WARDROBE");
        }

    }


    public void InteractObj()
    {

        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            if (player.currentState == PlayerState.Hiding)
            {
                player.ExitHiding(outsideWardrobe.position);
            }
            else
            {
                player.EnterHiding(insideWardrobe.position);
            }
        }
    }

}
