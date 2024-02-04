using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public string ItemName {get => itemName;}
    public string Description {get => description;}
    public Sprite Icon {get => icon;}

    public virtual bool Use(Pokemon target)
    {
        return false;
    }
}
