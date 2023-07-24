using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMove
{
    public PokemonMoveBase MoveBase{get;set;}
    public int MaxPP {get;set;}
    public int CurPP {get;set;}
    public bool Disabled {get;set;}

    public PokemonMove(PokemonMoveBase mBase)
    {
        MoveBase = mBase;
        MaxPP = MoveBase.PP;
        CurPP = MoveBase.PP;
        Disabled = false;
    }

    public void DecrementPP(bool pressure)
    {
        if(pressure)
        {
            CurPP-=2;
        }
        else
        {
            CurPP--;
        }
        
        if(CurPP < 0)
        {
            CurPP = 0;
        }
    }
}
