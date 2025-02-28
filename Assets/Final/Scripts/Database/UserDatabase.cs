using System;

public class UserDatabase
{
    public int ID { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime Date { get; set; }
    public DateTime Hour { get; set; }

    public UserDatabase(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
        Date = DateTime.Now; // Fecha actual
        Hour = DateTime.Now; // Hora actual
    }
}
