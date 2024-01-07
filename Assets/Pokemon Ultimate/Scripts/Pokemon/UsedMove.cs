using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedMove
{
    public PokemonMove Move{get;set;}
    public PokemonType MoveType{get;set;}

    public UsedMove(PokemonMove move)
    {
        MoveType = move.MoveBase.MoveType;
    }
}
