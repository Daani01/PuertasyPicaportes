using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    public float moveDistance = 0.5f; // Distancia que se moverá el cajón en el eje X
    public float moveDuration = 0.5f; // Duración del movimiento
    private bool isOpen = false; // Estado del cajón (abierto o cerrado)
    private bool isMoving = false; // Evitar múltiples interacciones mientras se mueve
    private AudioSource audioSource;

    public void InteractObj()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveDrawer());
        }
    }

    private IEnumerator MoveDrawer()
    {
        isMoving = true;

        // Determinar el sonido correcto
        string soundToPlay = isOpen ? "Close_Drawer" : "Open_Drawer";
        audioSource = SoundPoolManager.Instance.PlaySound(soundToPlay, gameObject);

        // Calcular posición inicial y final
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = isOpen
            ? startPosition - new Vector3(-moveDistance, 0, 0) // Cerrar el cajón
            : startPosition + new Vector3(-moveDistance, 0, 0); // Abrir el cajón

        // Animar el movimiento
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de llegar exactamente al destino
        transform.localPosition = targetPosition;

        // Alternar el estado del cajón
        isOpen = !isOpen;
        isMoving = false;
    }
}
