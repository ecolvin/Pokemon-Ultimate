using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // public SerializableDictionary<string, Vector3> characterPositions;
    // public SerializableDictionary<string, Quaternion> characterRotations;
    public SerializableDictionary<string, bool> trainersBattled;
    public SerializableDictionary<string, bool> groundItemsObtained;
    public SerializableDictionary<string, bool> groundTMsObtained;
    public PlayerSaveData player;

    public GameData()
    {
        // characterPositions = new SerializableDictionary<string, Vector3>();
        // characterRotations = new SerializableDictionary<string, Quaternion>();
        trainersBattled = new SerializableDictionary<string, bool>();
        groundItemsObtained = new SerializableDictionary<string, bool>();
        groundTMsObtained = new SerializableDictionary<string, bool>();
        player = null;
    }
}
