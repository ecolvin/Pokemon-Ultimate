using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildSpawning : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Reference to a shiny sparkle ParticleSystem")]
    [SerializeField] ParticleSystem shinySparkle;

    [Header("Game Rules")]
    [Tooltip("The chance of a grass patch producing an encounter when you walk into it")]
    [SerializeField] [Range(0,100)] int encounterOdds = 15;
    [Tooltip("The base chance of an encounter being a shiny pokemon")]
    [SerializeField] int baseShinyOdds = 4096;

    System.Random rand = new System.Random();  //TODO: Change to Unity Random

    string[] testSpawnTable = new string[]
    {
        "Pidgey"    , "Pidgey"    , "Pidgey"    , "Pidgey", 
        "Rattata"   , "Rattata"   , "Rattata"   , 
        "Oddish"    , "Oddish"    , "Oddish"    , 
        "Bellsprout", "Bellsprout", "Bellsprout"
    };

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Spawner" && rand.Next(100) < encounterOdds)
        {
            generateEncounter();
        }    
    }

    void generateEncounter()
    {
        string pokemonName = determineSpecies();
        bool isShiny = determineShiny();

        if(isShiny)
        {
            Debug.Log($"A wild SHINY {pokemonName} attacked!");
            shinySparkle.Play();
        }
        else
        {            
            Debug.Log($"A wild {pokemonName} attacked!");            
            shinySparkle.Stop();
        }
    }

    string determineSpecies()
    {
        int tableSize = testSpawnTable.Length;
        int speciesIndex = (int) rand.Next(tableSize);

        return testSpawnTable[speciesIndex];
    }

    bool determineShiny()
    {
        int numRolls = getShinyRolls();

        for(int i = 0; i < numRolls; i++)
        {
            if(rand.Next(baseShinyOdds) < 1)
            {
                return true;
            }
        }
        return false;
    }

    int getShinyRolls()
    {
        int numRolls = 1;
        // if(shinyCharm)
        // {
        //     numRolls += 3;
        // }
        return numRolls;
    }
}
