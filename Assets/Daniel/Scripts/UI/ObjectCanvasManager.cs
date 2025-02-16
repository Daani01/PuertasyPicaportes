using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCanvasManager : MonoBehaviour
{
    public Transform contentPanel; // Panel donde se añadirán los elementos
    public GameObject itemPrefab;
    public Font font; // Fuente para el texto
    public ItemData[] itemsData; // Array de datos predefinidos

    private List<GameObject> createdItems = new List<GameObject>(); // Lista de elementos creados

    public void AddItem(ItemType type, string value)
    {
        // Buscar en el array el item con el enum que se pasó
        ItemData selectedItem = System.Array.Find(itemsData, item => item.type == type);
        if (selectedItem != null)
        {
            GameObject newItem = CreateItemPrefab(selectedItem.image, value);
            newItem.transform.SetParent(contentPanel, false);
            createdItems.Add(newItem);
        }
        else
        {
            Debug.LogWarning($"No se encontró el item con tipo {type}");
        }
    }

    private GameObject CreateItemPrefab(Texture img, string textValue)
    {
        // Instanciar el prefab ya existente
        GameObject item = Instantiate(itemPrefab, contentPanel); // Asegúrate de que itemPrefab está asignado en el Inspector

        // Buscar y modificar la imagen
        RawImage image = item.transform.Find("Image").GetComponent<RawImage>();
        if (image != null)
        {
            image.texture = img;
        }

        // Buscar y modificar el texto
        TMP_Text text = item.transform.Find("Text").GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = textValue;
        }

        Slider slider = item.transform.Find("Slider").GetComponent<Slider>();
        if (slider != null)
        {
            //slider.value = ;
        }

        return item;
    }

    public void RemoveItem(int index)
    {
        // Verificar si el índice es válido
        if (index >= 0 && index < createdItems.Count)
        {
            // Eliminar el objeto de la lista y destruirlo
            GameObject itemToRemove = createdItems[index];
            createdItems.RemoveAt(index);
            Destroy(itemToRemove);

            // Reordenar los textos de los elementos restantes
            UpdateItemNumbers();
        }
        else
        {
            Debug.LogWarning($"Índice {index} fuera de rango. No se pudo eliminar el elemento.");
        }
    }

    // Función para actualizar los textos de los elementos restantes
    private void UpdateItemNumbers()
    {
        for (int i = 0; i < createdItems.Count; i++)
        {
            TMP_Text textComponent = createdItems[i].transform.Find("Text").GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = (i + 1).ToString(); // Se numera desde 1 hasta el total
            }
        }
    }





}
