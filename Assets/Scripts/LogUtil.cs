using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogUtil : MonoBehaviour
{

    private static string logFilePath;

    public static void SendLog(DataLog dataLog)
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "data_logs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string formattedDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

        dataLog.timePlayed = formattedDateTime;

        string json = JsonUtility.ToJson(dataLog);

        string fileName = string.Format("{0}_datalog.json", dataLog.timePlayed.Replace("-", "").Replace("T", "_").Replace(":", "").Replace("Z", ""));

        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log(filePath);

        using (StreamWriter writer = new (filePath))
        {
            writer.Write(json);
        }

    }

    public static void SendLogCSV(DataLog dataLog)
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "data_logs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        logFilePath = Path.Combine(folderPath, "data_logs.csv");

        // Escreva o cabeçalho do CSV se o arquivo não existir
        if (!File.Exists(logFilePath))
        {
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine("timePlayed,status"); // Adicione os cabeçalhos necessários aqui
            }
        }

        string formattedDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        dataLog.timePlayed = formattedDateTime;

        // Construa a linha CSV
        string csvLine = string.Format("{0},{1}", dataLog.timePlayed, dataLog.status);

        // Adicione a linha ao arquivo CSV
        using (StreamWriter writer = File.AppendText(logFilePath))
        {
            writer.WriteLine(csvLine);
        }

    }

}
