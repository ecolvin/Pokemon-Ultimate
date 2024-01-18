using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    [SerializeField] PokemonSpecies[] species = new PokemonSpecies [6]; 
    List<Pokemon> partyPokemon = new List<Pokemon>();
    public List<Pokemon> PartyPokemon{get{return partyPokemon;}}


    void Awake() 
    {
        partyPokemon.Add(new Pokemon(species[0], 5, true, true, false, true)); 
        partyPokemon.Add(new Pokemon(species[1], 5, true, true, false, true));
        partyPokemon.Add(new Pokemon(species[2], 5, true, true, false, true));
        //partyPokemon.Add(new Pokemon(species[3], 5, true, true, false));
        //partyPokemon.Add(new Pokemon(species[4], 5, true, true, false));
        //partyPokemon.Add(new Pokemon(species[5], 5, true, true, false));   
    }

    public Pokemon GetLeadPokemon()
    {
        foreach(Pokemon p in partyPokemon)
        {
            if(!p.Fainted)
            {
                return p;
            }
        }
        return null;
    }
}
