using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ObjectCanvasManager : MonoBehaviour
{
    public Transform contentPanel; // Panel donde se añadirán los elementos
    public GameObject itemPrefab;
    public Font font; // Fuente para el texto
    public ItemData[] itemsData; // Array de datos predefinidos

    private List<GameObject> createdItems = new List<GameObject>(); // Lista de elementos creados
    private List<Image> energyImages = new List<Image>(); // Lista para almacenar las imágenes de energía
    private List<IUsable> itemScripts = new List<IUsable>(); // Lista para almacenar las referencias de los scripts que contienen getEnergy

    public void AddItem(IUsable type, string value, bool energy)
    {
        // Buscar el item correspondiente en el array de datos predefinidos por el nombre del tipo
        ItemData selectedItem = System.Array.Find(itemsData, item => item.type == type.GetName());
        if (selectedItem != null)
        {
            // Crear el item a partir del prefab
            GameObject newItem = CreateItemPrefab(type, selectedItem.image, value, energy);
            newItem.transform.SetParent(contentPanel, false);
            createdItems.Add(newItem);
        }
        else
        {
            Debug.LogWarning($"No se encontró el item con tipo {type.GetName()}");
        }
    }

    private GameObject CreateItemPrefab(IUsable obj, Texture img, string textValue, bool energy)
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
        TMP_Text text = item.transform.Find("Number_Box").GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = textValue;
        }

        // Buscar y modificar el estado de Energy_Indicator
        GameObject energy_indicator = item.transform.Find("Energy_Indicator")?.gameObject;
        if (energy_indicator != null)
        {
            energy_indicator.SetActive(energy);

            // Asignar el IUsable a la lista
            IUsable itemScript = obj;  // Directamente usamos el parámetro obj que es de tipo IUsable
            if (itemScript != null)
            {
                // Guardar las referencias a los componentes para actualizarlos más tarde
                Image energyImage = energy_indicator.GetComponent<Image>();
                if (energyImage != null)
                {
                    energyImages.Add(energyImage); // Añadir a la lista
                    itemScripts.Add(itemScript);  // Añadir a la lista de scripts
                }
            }
        }

        return item;
    }

    // Actualizamos las barras de energía en cada frame
    void Update()
    {
        for (int i = 0; i < energyImages.Count; i++)
        {
            // Obtenemos el valor de energía de cada objeto
            if (itemScripts[i] != null)
            {
                float energyValue = itemScripts[i].getEnergy();  // Obtenemos el valor de la energía
                float maxEnergy = itemScripts[i].getMaxEnergy(); // Obtenemos el valor máximo de energía del objeto

                // Aseguramos que el valor de energía esté dentro de un rango válido
                energyValue = Mathf.Clamp(energyValue, 0, maxEnergy);

                // Mapeamos el valor de energía de [0, maxEnergy] a [0, 1] para el fillAmount
                float fillAmount = energyValue / maxEnergy;

                // Actualizamos el fillAmount de la imagen de energía
                energyImages[i].fillAmount = fillAmount;
            }
        }
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

    public void RemoveAllItems()
    {
        for (int i = createdItems.Count - 1; i >= 0; i--)
        {
            if (createdItems[i] != null)
            {
                Destroy(createdItems[i]);
                createdItems.RemoveAt(i);
            }
        }

        UpdateItemNumbers(); // Si querés actualizar la UI después
    }


    // Función para actualizar los textos de los elementos restantes
    private void UpdateItemNumbers()
    {
        for (int i = 0; i < createdItems.Count; i++)
        {
            TMP_Text textComponent = createdItems[i].transform.Find("Number_Box").GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = (i + 1).ToString(); // Se numera desde 1 hasta el total
            }
        }
    }
}
