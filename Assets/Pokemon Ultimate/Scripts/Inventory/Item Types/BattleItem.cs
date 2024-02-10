using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Battle Item")]
public class BattleItem : ItemBase
{
    
    public override bool CanUseOutsideBattle => false;
}