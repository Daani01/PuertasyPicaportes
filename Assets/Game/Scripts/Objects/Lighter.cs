using UnityEngine;
using System.Collections;
using static IUsable;

public class Lighter : MonoBehaviour, IUsable
{
    public ActivationType activationType;
    public GameObject Flashlighter;
    private float maxenergyTime; //(3 minutos)
    private float energyTime; //(3 minutos)
    private bool activatedLighter;
    private Coroutine lightCoroutine;

    private AudioSource audioSourceActivate;
    private AudioSource audioSourceDesactivate;
    private string soundNameActivate;
    private string soundNameDesctivate;

    private void Awake()
    {
        energyTime = float.Parse(CSVManager.Instance.GetSpecificData("Lighter", "EnergyTime"));
        maxenergyTime = float.Parse(CSVManager.Instance.GetSpecificData("Lighter", "MaxEnergyTime"));
        soundNameActivate = "Lighter_On";
        soundNameDesctivate = "Lighter_Off";
    }

    private void Start()
    {
        energyTime = maxenergyTime;
    }

    ItemType IUsable.GetName()
    {
        return ItemType.Lighter;
    }

    public bool Energy()
    {
        return true;
    }

    public float getEnergy()
    {
        if (Energy())
        {
            return energyTime;
        }
        else
        {
            return 0;
        }
    }

    public float getMaxEnergy()
    {
        if (Energy())
        {
            return maxenergyTime;
        }
        else
        {
            return 0;
        }
    }

    public void GetObjPlayer(Transform position, Transform lookat)
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

                        audioSourceActivate = SoundPoolManager.Instance.PlaySound(soundNameActivate, gameObject);
                        if (audioSourceActivate != null && audioSourceActivate.clip != null)
                        {
                            StartCoroutine(ReturnSoundToPool(audioSourceActivate.clip.length, soundNameActivate, audioSourceActivate));
                        }
                    }
                }
                else
                {
                    StopCoroutine(lightCoroutine);
                    lightCoroutine = null;

                    audioSourceDesactivate = SoundPoolManager.Instance.PlaySound(soundNameDesctivate, gameObject);
                    if (audioSourceDesactivate != null && audioSourceDesactivate.clip != null)
                    {
                        StartCoroutine(ReturnSoundToPool(audioSourceDesactivate.clip.length, soundNameDesctivate, audioSourceDesactivate));
                    }
                }
                break;

            case ActivationType.Charge:                
                break;
        }
    }

    private IEnumerator LighterTimer()
    {
        while (energyTime > 0)
        {
            energyTime -= Time.deltaTime;
            //Debug.Log("MECHERO: " + lighterTime.ToString());
            yield return null;
        }

        Destroy();
    }

    private IEnumerator ReturnSoundToPool(float delay, string name, AudioSource audio)
    {
        yield return new WaitForSeconds(delay);
        SoundPoolManager.Instance.ReturnToPool(name, audio);
    }
}
