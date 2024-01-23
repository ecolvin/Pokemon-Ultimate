using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCBox : MonoBehaviour
{
    Pokemon [] pokemon;
    int boxSize;

    public PCBox(int size)
    {
        pokemon = new Pokemon[size];
        boxSize = size;
    }

    public bool IsFull()
    {
        foreach(Pokemon p in pokemon)
        {
            if(p == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool AddPokemon(Pokemon poke)
    {
        if(IsFull())
        {
            Debug.Log("Box is full. AddPokemon() should not have been called");
            return false;
        }

        for(int i = 0; i < boxSize; i++)
        {
            if(pokemon[i] == null)
            {
                pokemon[i] = poke;                
                return true;
            }
        }

        Debug.Log("Box isn't full but you couldn't find a spot for the Pokemon (Something went wrong!)!");
        return false;
    }

    //0 = Success; 1 = Slot Taken; 2 = Box Full; 3 = Index out of bounds 
    public int AddPokemon(Pokemon poke, int i)
    {
        if(i >= boxSize)
        {
            Debug.Log($"Index {i} is not valid given the current boxSize: {boxSize}!");
            return 3;
        }        
        if(IsFull())
        {
            Debug.Log($"Box is full. AddPokemon({i}) should not have been called!");
            return 2;
        }
        if(pokemon[i] != null)
        {
            Debug.Log($"Slot {i} isn't empty. Pokemon cannot be put here");
            return 1;
        }
        pokemon[i] = poke;
        return 0;
    }

    public Pokemon[] GetAllPokemon()
    {
        return pokemon;
    }

    public Pokemon GetPokemon(int i)
    {
        return pokemon[i];
    }
}
