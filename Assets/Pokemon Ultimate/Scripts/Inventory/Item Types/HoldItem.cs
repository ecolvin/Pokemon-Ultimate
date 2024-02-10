using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Hold Item")]
public class HoldItem : ItemBase
{

    public override bool CanUseInBattle => false;
}
