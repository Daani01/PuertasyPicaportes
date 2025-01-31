using System.Collections.Generic;
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

    public void ClearItems()
    {
        foreach (GameObject item in createdItems)
        {
            Destroy(item);
        }
        createdItems.Clear();
    }

    private GameObject CreateItemPrefab(Sprite img, string textValue)
    {
        // Instanciar el prefab ya existente
        GameObject item = Instantiate(itemPrefab, contentPanel); // Asegúrate de que itemPrefab está asignado en el Inspector

        // Buscar y modificar la imagen
        Image image = item.transform.Find("Image").GetComponent<Image>();
        if (image != null)
        {
            image.sprite = img;
        }

        // Buscar y modificar el texto
        Text text = item.transform.Find("Text").GetComponent<Text>();
        if (text != null)
        {
            text.text = textValue;
        }

        return item;
    }

}
