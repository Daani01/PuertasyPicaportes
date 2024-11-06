using UnityEngine;
using System.Collections;
using static IUsable;

public class Lighter : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    public GameObject Flashlighter;
    public float lighterTime = 180f; //(3 minutos)
    private bool activatedLighter;
    private Coroutine lightCoroutine;

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
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }
        lightCoroutine = null;
        activatedLighter = false;
        Flashlighter.SetActive(false);
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
                break;

            case ActivationType.MultipleTimes:
                activatedLighter = !activatedLighter;
                Flashlighter.SetActive(activatedLighter);

                if (activatedLighter)
                {
                    if (lightCoroutine == null)
                    {
                        lightCoroutine = StartCoroutine(LighterTimer());
                    }
                }
                else
                {
                    StopCoroutine(lightCoroutine);
                    lightCoroutine = null;
                }
                break;

            case ActivationType.Charge:                
                break;
        }
    }

    private IEnumerator LighterTimer()
    {
        while (lighterTime > 0)
        {
            lighterTime -= Time.deltaTime;
            Debug.Log("MECHERO: " + lighterTime.ToString());
            yield return null;
        }

        // Apaga la linterna autom�ticamente cuando se acabe el tiempo
        DesActivate();
    }
}
