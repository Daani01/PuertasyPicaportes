using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    public float moveDistance = 0.5f; // Distancia que se mover� el caj�n en el eje X
    public float moveDuration = 0.5f; // Duraci�n del movimiento
    private bool isOpen = false; // Estado del caj�n (abierto o cerrado)
    private bool isMoving = false; // Evitar m�ltiples interacciones mientras se mueve
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

        // Calcular posici�n inicial y final
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = isOpen
            ? startPosition - new Vector3(-moveDistance, 0, 0) // Cerrar el caj�n
            : startPosition + new Vector3(-moveDistance, 0, 0); // Abrir el caj�n

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

        // Alternar el estado del caj�n
        isOpen = !isOpen;
        isMoving = false;
    }
}
