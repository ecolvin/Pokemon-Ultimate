using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] int gridSize = 10;
    public int GridSize {get => gridSize;}

    public static GlobalSettings Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }
}
