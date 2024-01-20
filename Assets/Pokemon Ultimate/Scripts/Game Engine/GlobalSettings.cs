using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleMode {Switch, Set};

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] int gridSize = 10;
    public int GridSize {get => gridSize;}
    [SerializeField] BattleMode battleMode = BattleMode.Switch;
    public BattleMode BattleMode {get => battleMode;}

    public static GlobalSettings Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }
}
