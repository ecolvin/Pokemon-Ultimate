using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // public SerializableDictionary<string, Vector3> characterPositions;
    // public SerializableDictionary<string, Quaternion> characterRotations;
    public SerializableDictionary<string, bool> trainersBattled;
    public PlayerSaveData player;

    public GameData()
    {
        // characterPositions = new SerializableDictionary<string, Vector3>();
        // characterRotations = new SerializableDictionary<string, Quaternion>();
        trainersBattled = new SerializableDictionary<string, bool>();
        player = null;
    }
}
