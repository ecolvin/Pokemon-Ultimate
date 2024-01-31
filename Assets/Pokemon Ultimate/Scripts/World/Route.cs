using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    [SerializeField] [NonReorderable] List<Encounter> encounterTable;       
    [Tooltip("The chance of a grass patch producing an encounter when you walk into it (percentage)")]
    [SerializeField] [Range(0,100)] int encounterOdds = 15;
    public int EncounterOdds {get => encounterOdds;}
    
    public Pokemon GetPokemon(Habitat habitat, TimePeriod time, int shinyRolls, int haRolls, int numPerfect)
    {
        List<Encounter> table = new List<Encounter>();
        foreach(Encounter enc in encounterTable)
        {
            if(enc.SpawnTimes.Contains(time) && enc.Habitats.Contains(habitat))
            {
                for(int i = 0; i < enc.Weight; i++)
                {
                    table.Add(enc);
                }
            }
        }
        
        int selection = Random.Range(0, table.Count);
        Encounter e = table[selection];
        int level = Random.Range(e.MinLevel, e.MaxLevel + 1);

        bool isShiny = true;
        bool isHA = true;
        //int numPerfect = 3;

        return new Pokemon(e.Species, level, isShiny, isHA, true, false);
    }
}

[System.Serializable]
public class Encounter
{
    [SerializeField] PokemonSpecies species;
    [SerializeField] int weight;
    [SerializeField] int minLevel;
    [SerializeField] int maxLevel;
    [SerializeField] List<TimePeriod> spawnTimes;
    [SerializeField] List<Habitat> habitats;

    public PokemonSpecies Species {get{return species;}}
    public int Weight {get{return weight;}}
    public int MinLevel {get{return minLevel;}}
    public int MaxLevel {get{return maxLevel;}}
    public List<TimePeriod> SpawnTimes {get{return spawnTimes;}}
    public List<Habitat> Habitats {get{return habitats;}}
}

public enum TimePeriod
{
    Morning,
    Day,
    Dusk,
    Night
}

public enum Habitat
{
    None,
    Grass,
    Surfing,
    Fishing
}