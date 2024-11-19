using UnityEngine;
using System.Collections;
using static IUsable;

public class Flashlight : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    public GameObject FlashlightLight;
    public float flashlightTime = 180f; //(3 minutos)
    private bool activatedFlashlight;
    private Coroutine flashlightCoroutine;

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
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DesActivate()
    {
        if (flashlightCoroutine != null)
        {
            StopCoroutine(flashlightCoroutine);
        }
        flashlightCoroutine = null;
        activatedFlashlight = false;
        FlashlightLight.SetActive(false);
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
                if (!activatedFlashlight)
                {
                    activatedFlashlight = true;
                    FlashlightLight.SetActive(true);
                    flashlightCoroutine = StartCoroutine(FlashlightTimer());
                }
                break;

            case ActivationType.MultipleTimes:
                activatedFlashlight = !activatedFlashlight;
                FlashlightLight.SetActive(activatedFlashlight);

                if (activatedFlashlight)
                {
                    if (flashlightCoroutine == null)
                    {
                        flashlightCoroutine = StartCoroutine(FlashlightTimer());
                    }
                }
                else
                {
                    StopCoroutine(flashlightCoroutine);
                    flashlightCoroutine = null;
                }
                break;

            case ActivationType.Charge:
                activatedFlashlight = !activatedFlashlight;
                FlashlightLight.SetActive(activatedFlashlight);

                if (activatedFlashlight)
                {
                    flashlightTime += 20f; //+20 segundos
                    if (flashlightCoroutine == null)
                    {
                        flashlightCoroutine = StartCoroutine(FlashlightTimer());
                    }
                }
                else
                {
                    StopCoroutine(flashlightCoroutine);
                    flashlightCoroutine = null;
                }
                break;
        }
    }

    private IEnumerator FlashlightTimer()
    {
        while (flashlightTime > 0)
        {
            flashlightTime -= Time.deltaTime;
            Debug.Log("LINTERNA: " + flashlightTime.ToString());
            yield return null;
        }

        Destroy();
    }
}
