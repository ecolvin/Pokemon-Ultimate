using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLoader : MonoBehaviour
{
    [SerializeField] GameObject globalPrefab;
    [SerializeField] GameObject spawnPoint;

    void Awake()
    {
        GlobalObjects [] existingGlobals = FindObjectsOfType<GlobalObjects>();
        if(existingGlobals.Length <= 0)
        {
            Instantiate(globalPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
    }
}
