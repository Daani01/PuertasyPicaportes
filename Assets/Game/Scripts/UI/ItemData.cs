using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Flashlight,
    Lighter,
    Crucifix,
    Pills
}


[System.Serializable]
public class ItemData
{
    public ItemType type; // Enum para identificar el item
    public Texture image; // Imagen del item
    public string text; // Texto del item
}
