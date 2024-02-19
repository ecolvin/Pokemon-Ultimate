using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IPlayerTrigger
{
    Route route;

    void Awake()
    {
        route = GetComponentInParent<Route>();
    }

    public IEnumerator OnPlayerTriggered(PlayerController player)
    {
        if(UnityEngine.Random.Range(0, 100) < route.EncounterOdds)
        {
            Pokemon p = route.GetPokemon(Habitat.Grass, TimePeriod.Day, GetShinyRolls(), 0, 0); //haRolls, numPerfect));
            yield return player.TriggerEncounter(p);
        }
    }
    
    int GetShinyRolls()
    {
        int numRolls = 1;
        // if(shinyCharm)
        // {
        //     numRolls += 3;
        // }
        return numRolls;
    }

}
