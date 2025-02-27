using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IUsable;

public class Pill : MonoBehaviour, IInteractable, IUsable
{
    public ActivationType activationType;
    public int SpeedTime;

    private AudioSource audioSource;
    private string soundName;

    public void InteractObj()
    {

    }

    ItemType IUsable.GetName()
    {
        return ItemType.Pills;
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
        soundName = "Pills";

        switch (activationType)
        {
            case ActivationType.OneTime:
                FirstPersonController player = FindObjectOfType<FirstPersonController>();

                if (player != null)
                {
                    audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);
                    if (audioSource != null && audioSource.clip != null)
                    {
                        StartCoroutine(ReturnSoundToPool(audioSource.clip.length, soundName, audioSource));
                    }

                    player.ActivatePillEffect(SpeedTime);
                    //Destroy();
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
        gameObject.gameObject.SetActive(false);

    }

}
