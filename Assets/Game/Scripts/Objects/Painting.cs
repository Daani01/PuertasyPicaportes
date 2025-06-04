using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    [Header("Texturas disponibles")]
    public Texture2D[] paintingTextures;

    [Header("Objeto con el material")]
    public Renderer targetRenderer;

    void Start()
    {
        if (paintingTextures.Length == 0 || targetRenderer == null)
        {
            Debug.LogWarning("Faltan texturas o el renderer no está asignado.");
            return;
        }

        // Elegir una textura aleatoria
        int index = Random.Range(0, paintingTextures.Length);
        Texture2D selectedTexture = paintingTextures[index];

        // Crear una nueva instancia del material para no afectar otros objetos
        Material newMaterial = new Material(targetRenderer.material);
        newMaterial.mainTexture = selectedTexture;
        targetRenderer.material = newMaterial;
    }
}
