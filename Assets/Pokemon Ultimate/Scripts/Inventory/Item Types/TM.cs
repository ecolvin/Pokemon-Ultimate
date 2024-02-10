using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/TM")]
public class TM : ItemBase
{
    [SerializeField] int number;
    [SerializeField] PokemonMoveBase move;
    
    public int Number {get => number;}
    public PokemonMoveBase Move => move;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUseInBattle => false;

}
