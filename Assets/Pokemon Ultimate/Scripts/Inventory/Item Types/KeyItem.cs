using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Key Item")]
public class KeyItem : ItemBase
{

    public override bool CanUseInBattle => false;
}
