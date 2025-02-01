using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVManager : MonoBehaviour, IProcess
{
    public TextAsset csvFile;

    private static CSVManager instance;
    private List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
    public bool IsCompleted { get; private set; } = false;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (csvFile != null)
            {
                LoadCSV(null); // Carga inicial si es necesario
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(LoadCSV(onComplete));
    }

    private IEnumerator LoadCSV(System.Action onComplete)
    {

        if (csvFile == null)
        {
            IsCompleted = true;
            onComplete?.Invoke();
            yield break;
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

        IsCompleted = true;
        onComplete?.Invoke();
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
