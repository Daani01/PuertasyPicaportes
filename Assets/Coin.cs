using UnityEngine;

public class Coin : MonoBehaviour, Interactable
{
    public int coinValue = 1;

    public void Interact()
    {
        CollectCoin();
    }

    private void CollectCoin()
    {
        // Aquí puedes incrementar el contador de monedas del jugador.
        Debug.Log("Moneda recogida. Valor: " + coinValue);
        //PlayerInventory.Instance.AddCoins(coinValue); // Suponiendo que hay un sistema de inventario.
        //Destroy(gameObject); // Elimina la moneda del juego.
    }
}
