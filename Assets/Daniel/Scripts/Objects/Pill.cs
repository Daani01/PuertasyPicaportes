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

    public void GetObjPlayer(Transform position)
    {
        gameObject.transform.position = position.position;
        gameObject.transform.rotation = position.rotation;
        gameObject.transform.SetParent(position.transform);
        gameObject.SetActive(false);
    }

    public void DesActivate()
    {        
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
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
                    DesActivate();
                }
                break;

            case ActivationType.MultipleTimes:                
                break;

            case ActivationType.Charge:                
                break;
        }
    }    

}
