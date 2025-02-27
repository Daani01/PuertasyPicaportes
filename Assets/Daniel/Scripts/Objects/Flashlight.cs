using UnityEngine;
using System.Collections;
using static IUsable;

public class Flashlight : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    public GameObject FlashlightLight;
    public float flashlightTime = 180f; //(3 minutos)

    private AudioSource audioSourceActivate;
    private AudioSource audioSourceDesactivate;
    private string soundNameActivate;
    private string soundNameDesctivate;

    private bool activatedFlashlight;
    private Coroutine flashlightCoroutine;

    public void InteractObj()
    {

    }

    ItemType IUsable.GetName()
    {
        return ItemType.Flashlight;
    }

    public void GetObjPlayer(Transform position, Transform lookat)
    {
        // Establecer la posición del objeto actual
        gameObject.transform.position = position.position;


        Vector3 lookDirection = new Vector3(lookat.position.x, transform.position.y, lookat.position.z);
        transform.LookAt(lookDirection);

        //añadir al eje x 90 grados
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 75, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

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
        soundNameActivate = "Flashlight_On";
        soundNameDesctivate = "Flashlight_Off";

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
                    flashlightTime += 20f; //+20 segundos
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
        while (flashlightTime > 0)
        {
            flashlightTime -= Time.deltaTime;
            Debug.Log("LINTERNA: " + flashlightTime.ToString());
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
