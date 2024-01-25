using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLoader : MonoBehaviour
{
    [SerializeField] GameObject globalPrefab;

    void Awake()
    {
        GlobalObjects [] existingGlobals = FindObjectsOfType<GlobalObjects>();
        if(existingGlobals.Length <= 0)
        {
            Instantiate(globalPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
    }
}
