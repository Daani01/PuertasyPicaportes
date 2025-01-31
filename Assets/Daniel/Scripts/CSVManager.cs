using UnityEngine;
using System.Collections.Generic;

public class CSVManager : MonoBehaviour
{
    public TextAsset csvFile;

    static private List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
    private static CSVManager instance;

    public static CSVManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CSVManager");
                instance = obj.AddComponent<CSVManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (csvFile != null)
        {
            LoadCSV();
        }
    }

    public void LoadCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("No se ha asignado un archivo CSV.");
            return;
        }

        data.Clear();
        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            Dictionary<string, string> row = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length; j++)
            {
                row[headers[j].Trim()] = values[j].Trim();
            }
            data.Add(row);
        }
    }

    public Dictionary<string, string> GetRowByEnemyName(string enemyName)
    {
        foreach (var row in data)
        {
            if (row.ContainsKey("Name") && row["Name"] == enemyName)
            {
                return row;
            }
        }
        return null;
    }

    public string GetSpecificData(string enemyName, string columnName)
    {
        Dictionary<string, string> row = GetRowByEnemyName(enemyName);
        if (row != null && row.ContainsKey(columnName))
        {
            return row[columnName];
        }
        return "Dato no encontrado";
    }
}
