using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static string filePath = Application.persistentDataPath + "/playerData.dat";
    private static string encryptionKey = "Xb8$2mLp&QzT1g#5";

    [Serializable]
    public class PlayerData
    {
        public int coins;
        public string recordTime;  // Formato HH:mm:ss
        public string globalTime;   // Formato HH:mm:ss (acumulado)
        public int attempts;
        public int deathsByRush;
        public int deathsByEyes;
        public int deathsByScreech;
        public int doorRecord;
    }

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        byte[] encryptedData = Encrypt(json, encryptionKey);
        File.WriteAllBytes(filePath, encryptedData);

        Debug.Log("Datos guardados en: " + filePath);
    }

    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No hay datos guardados.");
            return null;
        }

        byte[] encryptedData = File.ReadAllBytes(filePath);
        string json = Decrypt(encryptedData, encryptionKey);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    private static byte[] Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    private static string Decrypt(byte[] cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];

            using (MemoryStream ms = new MemoryStream(cipherText))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
