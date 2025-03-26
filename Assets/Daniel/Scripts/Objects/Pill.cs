using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IUsable;

public class Pill : MonoBehaviour, IUsable
{
    public ActivationType activationType;
    private int speedAmount;

    private AudioSource audioSource;
    private string soundName;

    private void Awake()
    {
        speedAmount = int.Parse(CSVManager.Instance.GetSpecificData("Pills", "Amount"));
        soundName = "Pills";
    }

    ItemType IUsable.GetName()
    {
        return ItemType.Pills;
    }

    public bool Energy()
    {
        return false;
    }

    public float getEnergy()
    {
        if (Energy())
        {
            return 1;
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
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void GetObjPlayer(Transform position, Transform lookat)
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
                    AudioSource audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);

                    if (audioSource != null)
                    {
                        // Desvincular el sonido del objeto Pill para que no se desactive con él
                        audioSource.transform.parent = null;

                        if (audioSource.clip != null)
                        {
                            StartCoroutine(ReturnSoundToPool(audioSource.clip.length, soundName, audioSource));
                        }
                    }

                    player.ActivatePillEffect(speedAmount);
                    Destroy();
                }
                break;

            case ActivationType.MultipleTimes:                
                break;

            case ActivationType.Charge:                
                break;
        }
    }

    private IEnumerator ReturnSoundToPool(float delay, string name, AudioSource audio)
    {
        yield return new WaitForSeconds(delay);
        SoundPoolManager.Instance.ReturnToPool(name, audio);
    }

}
