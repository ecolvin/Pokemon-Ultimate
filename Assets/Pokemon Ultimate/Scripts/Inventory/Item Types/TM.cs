using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/TM")]
public class TM : ItemBase
{
    [SerializeField] int number;
    [SerializeField] PokemonMoveBase move;
    public PokemonMoveBase Move => move;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
