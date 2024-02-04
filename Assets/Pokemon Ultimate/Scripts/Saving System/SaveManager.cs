using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    [SerializeField] string filename;

    GameData gameData;
    DataHandler dataHandler;
    List<ISaveable> saveableObjects;

    public static SaveManager Instance {get; private set;}

    void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        Instance = this;
    }

    void Start()
    {
        dataHandler = new DataHandler(Application.persistentDataPath, filename);
        saveableObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if(gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
            return;
        }

        saveableObjects = FindAllDataPersistenceObjects();

        foreach (ISaveable saveableObj in saveableObjects)
        {
            saveableObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        saveableObjects = FindAllDataPersistenceObjects();
        
        foreach (ISaveable saveableObj in saveableObjects)
        {
            saveableObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    List<ISaveable> FindAllDataPersistenceObjects()
    {
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }
    
    public void SaveDataForList(List<ISaveable> objects)
    {
        if(objects == null)
        {
            return;
        }

        foreach (ISaveable saveableObj in objects)
        {
            saveableObj.SaveData(ref gameData);
        }
    }

    public void LoadDataForList(List<ISaveable> objects)
    {
        if(objects == null)
        {
            return;
        }
        
        foreach (ISaveable saveableObj in objects)
        {
            saveableObj.LoadData(gameData);
        }
    }
}
