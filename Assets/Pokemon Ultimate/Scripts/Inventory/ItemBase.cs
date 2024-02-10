using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] InventoryTab tab;
    [SerializeField] InvUIState uiState;

    public string ItemName {get => itemName;}
    public string Description {get => description;}
    public Sprite Icon {get => icon;}
    public InventoryTab Tab {get => tab;}
    public InvUIState UIState {get => uiState;}

    public virtual bool Use(Pokemon target)
    {
        return false;
    }

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
