using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class A90Controller : Enemie
{
    public Image A90IDLE;
    public Image A90BLOCK;
    public Image A90BACK;
    public AudioClip A90AUDIO;
    private FirstPersonController player;
    private bool hasDetectedMovement = false;

    private void Awake()
    {
        damage = 90;
        dieInfo = "NO TE MUEVAS PAI!!!";

        if (A90IDLE == null)
            A90IDLE = GameObject.Find("A90IDLECanvas").GetComponent<Image>();
        if (A90BLOCK == null)
            A90BLOCK = GameObject.Find("A90BLOCKCanvas").GetComponent<Image>();
        if (A90BACK == null)
            A90BACK = GameObject.Find("A90BACKCanvas").GetComponent<Image>();
        player = GameObject.FindAnyObjectByType<FirstPersonController>();
    }

    private void OnEnable()
    {
        StartCoroutine(HandleA90Sequence());
    }

    private IEnumerator HandleA90Sequence()
    {
        SetImageAlpha(A90IDLE, 1f);
        yield return new WaitForSeconds(0.3f);

        SetImageAlpha(A90IDLE, 0f);
        SetImageAlpha(A90BLOCK, 1f);
        SetImageAlpha(A90BACK, 1f);

        // Detectar movimiento o rotación del jugador una sola vez
        while (!hasDetectedMovement)
        {
            if (player != null && (player.moveInput.magnitude > 0f || player.lookInput.magnitude > 0f))
            {
                player.TakeDamage(damage, gameObject.GetComponent<Enemie>());
                hasDetectedMovement = true;
            }
            yield return null; // Espera un frame antes de volver a comprobar
        }

        float audioDuration = A90AUDIO.length - 0.3f;
        yield return new WaitForSeconds(audioDuration);

        SetImageAlpha(A90BLOCK, 0f);
        SetImageAlpha(A90BACK, 0f);

        gameObject.SetActive(false);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color tempColor = image.color;
            tempColor.a = alpha;
            image.color = tempColor;
        }
    }
}
