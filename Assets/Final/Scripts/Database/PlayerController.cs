using TMPro;
using UnityEngine;
using System.Threading.Tasks; // Necesario para usar async y await

public class PlayerController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    private DatabaseManager dbManager;

    private void Start()
    {
        dbManager = new DatabaseManager();
    }

    // M�todo asincr�nico para insertar un nuevo jugador
    public async void InsertPlayer()
    {
        // Recoger los datos del formulario
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        // Crear un nuevo objeto Player
        UserDatabase newPlayer = new UserDatabase(username, email, password);

        // Insertar en la base de datos (llamando al m�todo asincr�nico)
        await dbManager.InsertPlayerAsync(newPlayer);
    }

    // M�todo asincr�nico para obtener un jugador por ID
    public async void GetPlayerById(int id)
    {
        // Esperar a que el jugador sea recuperado asincr�nicamente
        UserDatabase player = await dbManager.GetPlayerByIdAsync(id);

        if (player != null)
        {
            Debug.Log("Player retrieved: " + player.Username);
        }
        else
        {
            Debug.LogError("Player not found");
        }
    }
}
