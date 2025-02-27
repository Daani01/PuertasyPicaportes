using UnityEngine;
using System.Collections;
using static FirstPersonController;

public class Coins : MonoBehaviour, IInteractable
{
    public int value;
    private AudioSource audioSource;
    private string soundName;

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.coinsCount += value;
            player.ShowMessage($"Has recibido: {value} monedas", 4f);

            soundName = "Coins";
            audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);

            if (audioSource != null && audioSource.clip != null)
            {
                StartCoroutine(ReturnSoundToPool(audioSource.clip.length));
            }
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private IEnumerator ReturnSoundToPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundPoolManager.Instance.ReturnToPool(soundName, audioSource);
        gameObject.SetActive(false);

    }
}
