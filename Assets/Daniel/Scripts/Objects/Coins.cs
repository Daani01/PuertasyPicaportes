using UnityEngine;
using System.Collections;
using static FirstPersonController;

public class Coins : MonoBehaviour, IInteractable
{
    public int value;
    private string soundName = "Coins";

    public void InteractObj()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.AddCoins(value);

            // Reproducir el sonido y obtener el AudioSource
            AudioSource audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);

            if (audioSource != null)
            {
                // Desvincular el AudioSource del objeto para que no se desactive junto con la moneda
                audioSource.transform.parent = null;

                // Si el clip es válido, devolverlo a la pool después de que termine el sonido
                if (audioSource.clip != null)
                {
                    StartCoroutine(ReturnSoundToPool(audioSource.clip.length, audioSource));
                }
            }

            // Desactivar el objeto inmediatamente para prevenir nuevas interacciones
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ReturnSoundToPool(float delay, AudioSource audioSource)
    {
        yield return new WaitForSeconds(delay);
        SoundPoolManager.Instance.ReturnToPool(soundName, audioSource);
    }
}
