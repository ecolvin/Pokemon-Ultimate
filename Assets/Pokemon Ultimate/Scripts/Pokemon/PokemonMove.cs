using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMove
{
    public PokemonMoveBase MoveBase{get;set;}
    public int CurPP {get;set;}
    public bool Disabled {get;set;}

    public PokemonMove(PokemonMoveBase mBase)
    {
        MoveBase = mBase;
        CurPP = MoveBase.PP;
        Disabled = false;
    }
}
