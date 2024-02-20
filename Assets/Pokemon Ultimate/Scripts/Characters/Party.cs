using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Party : MonoBehaviour
{
    [SerializeField] PartyPokemon[] partyPokemon = new PartyPokemon [6]; 
    List<Pokemon> pokemon = new List<Pokemon>();
    public List<Pokemon> Pokemon{get{return pokemon;}set{pokemon = value; OnUpdated?.Invoke();}}

    public event Action OnUpdated;

    public static Party GetParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Party>();
    }

    void Awake() 
    {
        for(int i = 0; i < partyPokemon.Length; i++)
        {
            Pokemon pkmn = partyPokemon[i].GetPokemon();
            if(pkmn != null)
            {
                pokemon.Add(pkmn);
            }
        }  
    }

    public Pokemon GetLeadPokemon()
    {
        foreach(Pokemon p in pokemon)
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

    public int NumHealthyPokemon()
    {
        int count = 0;
        foreach(Pokemon p in pokemon)
        {
            if(p == null)
            {
                continue;
            }
            if(!p.Fainted)
            {
                count++;
            }
        }
        return count;
    }

    public void EndBattle()
    {
        foreach(Pokemon p in pokemon)
        {
            p.EndBattle();
        }
    }

    bool IsFull()
    {
        return !(pokemon.Count < 6);
    }

    public bool AddPokemon(Pokemon p)
    {
        if(IsFull() || p == null)
        {
            return false;
        }
        pokemon.Add(p);
        OnUpdated?.Invoke();
        return true;
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach(Pokemon p in pokemon)
        {
            Evolution evo = p.CheckForLvlUpEvo();
            if(evo != null)
            {
                yield return EvolutionHandler.Instance.Evolve(p, evo);
                OnUpdated?.Invoke();
            }
        }
        yield return null;
    }

    public void UpdateParty()
    {
        OnUpdated?.Invoke();
    }
}
