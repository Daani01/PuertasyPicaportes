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

    ItemType IUsable.GetName()
    {
        return ItemType.Lighter;
    }
    public void GetObjPlayer(Transform position)
    {
        // Establecer la posición del objeto actual
        gameObject.transform.position = position.position;

        // Configurar la rotación para que mire hacia el eje Z del transform pasado
        Vector3 direction = position.forward; // Dirección del eje Z del transform pasado
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up); // Calcular la rotación
        gameObject.transform.rotation = rotation;

        // Establecer el transform actual como hijo del transform pasado
        gameObject.transform.SetParent(position);

        // Desactivar el objeto
        gameObject.SetActive(false);
    }


    public void Activate()
    {
        gameObject.SetActive(true);
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

        Destroy();
    }
}
