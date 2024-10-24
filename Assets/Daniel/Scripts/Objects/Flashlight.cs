using UnityEngine;
using static IUsable;

public class Flashlight : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;

    public void InteractObj()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        gameObject.SetActive(false);
    }

    public void Use()
    {
        switch (activationType)
        {
            case ActivationType.OneTime:
                Debug.Log(" used once.");
                break;
            case ActivationType.MultipleTimes:
                Debug.Log(" can be used multiple times.");
                break;
            case ActivationType.Charge:
                Debug.Log(" is charging.");
                break;
        }
    }
}
