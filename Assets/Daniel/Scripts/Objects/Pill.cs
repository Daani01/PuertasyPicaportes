using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IUsable;

public class Pill : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    public int SpeedTime;

    public void InteractObj()
    {

    }

    ItemType IUsable.GetName()
    {
        return ItemType.Pills;
    }
    public void GetObjPlayer(Transform position)
    {
        gameObject.transform.position = position.position;
        gameObject.transform.rotation = position.rotation;
        gameObject.transform.SetParent(position.transform);
        gameObject.SetActive(false);
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DesActivate()
    {        
        gameObject.SetActive(false);
    }    

    public void Destroy()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.RemoveItem(this);
            DesActivate();
        }
    }

    public void Use()
    {
        switch (activationType)
        {
            case ActivationType.OneTime:
                FirstPersonController player = FindObjectOfType<FirstPersonController>();

                if (player != null)
                {
                    player.ActivatePillEffect(SpeedTime);
                    Destroy();
                }
                break;

            case ActivationType.MultipleTimes:                
                break;

            case ActivationType.Charge:                
                break;
        }
    }    

    

}
