using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataHandler
{
    string saveDataPath = "";
    string saveDataFilename = "";

    public DataHandler(string path, string filename)
    {
        saveDataPath = path;
        saveDataFilename = filename;
    }

    public GameData Load()
    {        
        string fullPath = Path.Combine(saveDataPath, saveDataFilename);
        GameData loadData = null;
        if(File.Exists(fullPath))
        {   
            try
            {
                string loadDataJSON = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        loadDataJSON = reader.ReadToEnd();
                    }
                }

                loadData = JsonUtility.FromJson<GameData>(loadDataJSON);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(saveDataPath, saveDataFilename);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string gameDataJSON = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(gameDataJSON);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
