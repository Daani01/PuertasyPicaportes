using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TransformConnectorGizmo : MonoBehaviour
{
    // El Transform al cual se quiere conectar
    public Transform targetTransform;

    // Color y tamaño del gizmo
    public Color gizmoColor = Color.red;
    public float sphereRadius = 0.2f;

    private void OnDrawGizmos()
    {
        // Dibujar una esfera en la posición de este Transform
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, sphereRadius);

        // Si el Transform objetivo está asignado, dibujar una línea hacia él
        if (targetTransform != null)
        {
            Gizmos.DrawLine(transform.position, targetTransform.position);

            // Dibujar las coordenadas de X y Z de este Transform y el objetivo
#if UNITY_EDITOR
            Handles.color = Color.white;
            Vector3 thisPosition = transform.position;
            Vector3 targetPosition = targetTransform.position;

            // Mostrar las coordenadas X y Z de este objeto
            Handles.Label(thisPosition + Vector3.up * 0.5f, $"X: {thisPosition.x:F2}, Z: {thisPosition.z:F2}", new GUIStyle { fontSize = 12, normal = { textColor = Color.cyan } });

            // Mostrar las coordenadas X y Z del objeto objetivo
            Handles.Label(targetPosition + Vector3.up * 0.5f, $"X: {targetPosition.x:F2}, Z: {targetPosition.z:F2}", new GUIStyle { fontSize = 12, normal = { textColor = Color.green } });
#endif
        }
    }
}
