using UnityEngine;
using System.Collections;
using static IUsable;

public class Flashlight : MonoBehaviour, IUsable
{
    public ActivationType activationType;
    public GameObject FlashlightLight;
    private float maxenergyTime; //(3 minutos)
    private float energyTime;

    private AudioSource audioSourceActivate;
    private AudioSource audioSourceDesactivate;
    private string soundNameActivate;
    private string soundNameDesctivate;

    private bool activatedFlashlight;
    private Coroutine flashlightCoroutine;

    private void Awake()
    {
        energyTime = float.Parse(CSVManager.Instance.GetSpecificData("Flashlight", "EnergyTime"));
        maxenergyTime = float.Parse(CSVManager.Instance.GetSpecificData("Flashlight", "MaxEnergyTime"));
        soundNameActivate = "Flashlight_On";
        soundNameDesctivate = "Flashlight_Off";
    }

    private void Start()
    {
        energyTime = maxenergyTime;
    }

    ItemType IUsable.GetName()
    {
        return ItemType.Flashlight;
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

        //-95, 3, 0
        //Vector3 lookDirection = new Vector3(lookat.position.x, transform.position.y, lookat.position.z);
        //transform.LookAt(lookDirection);

        //añadir al eje x 90 grados
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 55, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);




        // Establecer el transform actual como hijo del transform pasado
        gameObject.transform.SetParent(position);


        gameObject.transform.localRotation = Quaternion.Euler(-95, -3, 0);

        // Desactivar el objeto
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

                    audioSourceActivate = SoundPoolManager.Instance.PlaySound(soundNameActivate, gameObject);
                    if (audioSourceActivate != null && audioSourceActivate.clip != null)
                    {
                        StartCoroutine(ReturnSoundToPool(audioSourceActivate.clip.length, soundNameActivate, audioSourceActivate));
                    }
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

                        audioSourceActivate = SoundPoolManager.Instance.PlaySound(soundNameActivate, gameObject);
                        if (audioSourceActivate != null && audioSourceActivate.clip != null)
                        {
                            StartCoroutine(ReturnSoundToPool(audioSourceActivate.clip.length, soundNameActivate, audioSourceActivate));
                        }
                    }
                }
                else
                {
                    StopCoroutine(flashlightCoroutine);
                    flashlightCoroutine = null;

                    audioSourceDesactivate = SoundPoolManager.Instance.PlaySound(soundNameDesctivate, gameObject);
                    if (audioSourceDesactivate != null && audioSourceDesactivate.clip != null)
                    {
                        StartCoroutine(ReturnSoundToPool(audioSourceDesactivate.clip.length, soundNameDesctivate, audioSourceDesactivate));
                    }
                }
                break;

            case ActivationType.Charge:
                activatedFlashlight = !activatedFlashlight;
                FlashlightLight.SetActive(activatedFlashlight);

                if (activatedFlashlight)
                {
                    // Comprobar si el tiempo de la linterna no excede el máximo permitido
                    if (energyTime + 2f <= maxenergyTime)
                    {
                        energyTime += 2f; // Añadir 2 segundos
                    }
                    else
                    {
                        energyTime = maxenergyTime; // Ajustar al máximo si se excede
                    }

                    // Iniciar la corrutina solo si es necesario
                    if (flashlightCoroutine == null)
                    {
                        flashlightCoroutine = StartCoroutine(FlashlightTimer());

                        audioSourceActivate = SoundPoolManager.Instance.PlaySound(soundNameActivate, gameObject);
                        if (audioSourceActivate != null && audioSourceActivate.clip != null)
                        {
                            StartCoroutine(ReturnSoundToPool(audioSourceActivate.clip.length, soundNameActivate, audioSourceActivate));
                        }
                    }
                }
                else
                {
                    StopCoroutine(flashlightCoroutine);
                    flashlightCoroutine = null;

                    audioSourceDesactivate = SoundPoolManager.Instance.PlaySound(soundNameDesctivate, gameObject);
                    if (audioSourceDesactivate != null && audioSourceDesactivate.clip != null)
                    {
                        StartCoroutine(ReturnSoundToPool(audioSourceDesactivate.clip.length, soundNameDesctivate, audioSourceDesactivate));
                    }
                }
                break;

        }
    }

    private IEnumerator FlashlightTimer()
    {
        while (energyTime > 0)
        {
            energyTime -= Time.deltaTime;
            //Debug.Log("LINTERNA: " + flashlightTime.ToString());
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
