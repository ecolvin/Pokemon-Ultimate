using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleMode {Switch, Set};

public class GlobalSettings : MonoBehaviour
{
    const int MAX_ITEM_QUANTITY = 999;
    public int MaxItemQuantity {get => MAX_ITEM_QUANTITY;}

    [SerializeField] int gridSize = 10;
    public int GridSize {get => gridSize;}
    [SerializeField] BattleMode battleMode = BattleMode.Switch;
    public BattleMode BattleMode {get => battleMode;}    
    [SerializeField] int baseShinyOdds = 4096;
    public int BaseShinyOdds {get => baseShinyOdds;}
    [SerializeField] Color defaultColor = Color.black;
    public Color DefaultColor {get => defaultColor;}
    [SerializeField] Color selectedColor = Color.red;
    public Color SelectedColor {get => selectedColor;}
    [SerializeField] Color selectedBarColor = new Color(255, 200, 0, 255);
    public Color SelectedBarColor {get => selectedBarColor;}


    public static GlobalSettings Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }
}
