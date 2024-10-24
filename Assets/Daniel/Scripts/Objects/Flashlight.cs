using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static IUsable;

public class Flashlight : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    bool activatedFlashlight;
    public GameObject FlashlightLight;
        
    public void InteractObj()
    {        

    }

    public void DesActivateObj(Transform position)
    {
        gameObject.transform.position = position.position;
        gameObject.transform.SetParent(position.transform, true);
        gameObject.SetActive(false);
    }

    public void Use()
    {
        gameObject.SetActive(true);

        switch (activationType)
        {
            case ActivationType.OneTime:

                break;
            case ActivationType.MultipleTimes:

                activatedFlashlight = !activatedFlashlight;
                FlashlightLight.SetActive(activatedFlashlight);

                break;
            case ActivationType.Charge:

                break;
        }
    }
}
