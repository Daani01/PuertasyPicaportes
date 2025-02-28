using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;  // Necesario para async y await
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString = "Server=sql.bsite.net\\MSSQL2016;database=doors;UID=daani01_;password=daani01_;";

    // M�todo para abrir la conexi�n asincr�nicamente
    private async Task<MySqlConnection> OpenConnectionAsync()
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            await connection.OpenAsync();  // Abrir la conexi�n de manera asincr�nica
            Debug.Log("Connection successful");
            return connection;
        }
        catch (Exception ex)
        {
            Debug.LogError("Connection failed: " + ex.Message);
            return null;
        }
    }

    // M�todo para cerrar la conexi�n asincr�nicamente
    private async Task CloseConnectionAsync(MySqlConnection connection)
    {
        try
        {
            await connection.CloseAsync();  // Cerrar la conexi�n de manera asincr�nica
            Debug.Log("Connection closed");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to close connection: " + ex.Message);
        }
    }

    // Insertar jugador asincr�nicamente
    public async Task InsertPlayerAsync(UserDatabase player)
    {
        string query = "INSERT INTO players (Username, Email, Password, Date, Hour) VALUES (@Username, @Email, @Password, NOW(), NOW())";
        MySqlConnection connection = await OpenConnectionAsync();  // Abrimos la conexi�n

        if (connection == null) return;  // Si no se pudo conectar, no hacemos nada

        try
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", player.Username);
            cmd.Parameters.AddWithValue("@Email", player.Email);
            cmd.Parameters.AddWithValue("@Password", player.Password);

            await cmd.ExecuteNonQueryAsync();  // Ejecutamos la consulta asincr�nicamente
            Debug.Log("Player inserted successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error inserting player: " + ex.Message);
        }
        finally
        {
            await CloseConnectionAsync(connection);  // Cerramos la conexi�n
        }
    }

    // Obtener jugador por ID asincr�nicamente
    public async Task<UserDatabase> GetPlayerByIdAsync(int id)
    {
        string query = "SELECT * FROM players WHERE ID = @ID";
        MySqlConnection connection = await OpenConnectionAsync();  // Abrimos la conexi�n

        if (connection == null) return null;  // Si no se pudo conectar, no hacemos nada

        UserDatabase player = null;

        try
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID", id);

            MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();  // Leemos los datos asincr�nicamente

            if (reader.Read())
            {
                int playerId = reader.GetInt32("ID");
                string username = reader.GetString("Username");
                string email = reader.GetString("Email");
                string password = reader.GetString("Password");
                DateTime date = reader.GetDateTime("Date");
                DateTime hour = reader.GetDateTime("Hour");

                player = new UserDatabase(username, email, password)
                {
                    ID = playerId,
                    Date = date,
                    Hour = hour
                };
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving player: " + ex.Message);
        }
        finally
        {
            await CloseConnectionAsync(connection);  // Cerramos la conexi�n
        }

        return player;
    }

    // Actualizar jugador asincr�nicamente
    public async Task UpdatePlayerAsync(UserDatabase player)
    {
        string query = "UPDATE players SET Username = @Username, Email = @Email, Password = @Password WHERE ID = @ID";
        MySqlConnection connection = await OpenConnectionAsync();  // Abrimos la conexi�n

        if (connection == null) return;  // Si no se pudo conectar, no hacemos nada

        try
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID", player.ID);
            cmd.Parameters.AddWithValue("@Username", player.Username);
            cmd.Parameters.AddWithValue("@Email", player.Email);
            cmd.Parameters.AddWithValue("@Password", player.Password);

            await cmd.ExecuteNonQueryAsync();  // Ejecutamos la consulta asincr�nicamente
            Debug.Log("Player updated successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating player: " + ex.Message);
        }
        finally
        {
            await CloseConnectionAsync(connection);  // Cerramos la conexi�n
        }
    }

    // Eliminar jugador asincr�nicamente
    public async Task DeletePlayerAsync(int id)
    {
        string query = "DELETE FROM players WHERE ID = @ID";
        MySqlConnection connection = await OpenConnectionAsync();  // Abrimos la conexi�n

        if (connection == null) return;  // Si no se pudo conectar, no hacemos nada

        try
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID", id);

            await cmd.ExecuteNonQueryAsync();  // Ejecutamos la consulta asincr�nicamente
            Debug.Log("Player deleted successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error deleting player: " + ex.Message);
        }
        finally
        {
            await CloseConnectionAsync(connection);  // Cerramos la conexi�n
        }
    }
}
