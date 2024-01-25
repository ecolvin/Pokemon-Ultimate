using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    [SerializeField] PartyPokemon[] pokemon = new PartyPokemon [6]; 
    List<Pokemon> partyPokemon = new List<Pokemon>();
    public List<Pokemon> PartyPokemon{get{return partyPokemon;}}


    void Awake() 
    {
        for(int i = 0; i < pokemon.Length; i++)
        {
            Pokemon pkmn = pokemon[i].GetPokemon();
            if(pkmn != null)
            {
                partyPokemon.Add(pkmn);
            }
        }  
    }

    public Pokemon GetLeadPokemon()
    {
        foreach(Pokemon p in partyPokemon)
        {
            if(p == null)
            {
                continue;
            }
            if(!p.Fainted)
            {
                return p;
            }
        }
        return null;
    }

    public void EndBattle()
    {
        foreach(Pokemon p in partyPokemon)
        {
            p.EndBattle();
        }
    }

    bool IsFull()
    {
        return !(partyPokemon.Count < 6);
    }

    public bool AddPokemon(Pokemon p)
    {
        if(IsFull())
        {
            return false;
        }
        partyPokemon.Add(p);
        return true;
    }
}
