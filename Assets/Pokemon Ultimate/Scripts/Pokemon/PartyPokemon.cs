using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnownMove
{
    [SerializeField] PokemonMoveBase move;
    public PokemonMoveBase Move{get => move;}
}

[System.Serializable]
public class PartyPokemon
{
    [SerializeField] PokemonSpecies species = null;
    [SerializeField] [Range(1,100)] int level = 1;
    [SerializeField] StatBlock evs = new StatBlock(0,0,0,0,0,0);
    [SerializeField] StatBlock ivs = new StatBlock(0,0,0,0,0,0);
    [SerializeField] PokemonNature nature = PokemonNature.None;
    [SerializeField] PokemonType teraType = PokemonType.None;
    [SerializeField] PokemonGender gender = PokemonGender.None;
    [SerializeField] string ability;
    [SerializeField] string heldItem;
    [SerializeField] bool isShiny = false;
    [SerializeField] bool isPlayer = false;
    [SerializeField] KnownMove [] moves = new KnownMove[4];

    public Pokemon GetPokemon()
    {
        if(species == null)
        {
            return null;
        }

        PokemonMove [] pokemonMoves = new PokemonMove[4];

        for(int i = 0; i < moves.Length; i++)
        {
            if(moves[i] == null)
            {
                continue;
            }
            if(moves[i].Move != null)
            {
                pokemonMoves[i] = new PokemonMove(moves[i].Move);
            }
        }

        return new Pokemon(species, level, evs, ivs, nature, teraType, gender, ability, heldItem, pokemonMoves, isShiny, false, isPlayer);
    }
}
