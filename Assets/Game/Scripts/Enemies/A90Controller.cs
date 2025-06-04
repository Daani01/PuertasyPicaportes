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
        yield return new WaitForSeconds(0.5f);

        SetImageAlpha(A90IDLE, 0f);
        SetImageAlpha(A90BLOCK, 1f);
        SetImageAlpha(A90BACK, 1f);

        float timer = 0f;
        float audioDuration = A90AUDIO.length - 0.5f;

        // Mientras el audio esté sonando
        while (timer < audioDuration)
        {
            if (!hasDetectedMovement && player != null && (player.moveInput.magnitude > 0f || player.lookInput.magnitude > 0f))
            {
                player.TakeDamage(damage, gameObject.GetComponent<Enemie>());
                hasDetectedMovement = true;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Si no se movió, se ejecuta esto después del audio
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
