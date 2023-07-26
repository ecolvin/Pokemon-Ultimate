using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] int gridSize = 10;
    [SerializeField] Transform body;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] LayerMask spawner;     
    
    [Tooltip("Reference to a shiny sparkle ParticleSystem")]
    [SerializeField] ParticleSystem shinySparkle;      
    [Tooltip("The chance of a grass patch producing an encounter when you walk into it (percentage)")]
    [SerializeField] [Range(0,100)] int encounterOdds = 15;    
    [Tooltip("The base chance of an encounter being a shiny pokemon")]
    [SerializeField] int baseShinyOdds = 4096;

    public event Action<int, int, int> OnEncounter;

    bool isMoving = false;

    Vector3 curPos;
    Vector3 newPos;

    float xOffset = 0;
    float zOffset = 0;

    void Start()
    {
        curPos = transform.position;
        newPos = transform.position;
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public void HandleUpdate()
    {
        movePlayer();        
    }

    void movePlayer()
    {
        if(!isMoving)
        {
            StartCoroutine(smoothGridMovement());
        }
    }

    IEnumerator smoothGridMovement()
    {        
        isMoving = true;
        curPos = transform.position;
        xOffset = Input.GetAxisRaw("Horizontal") * gridSize;
        zOffset = Input.GetAxisRaw("Vertical") * gridSize;
        newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
        if(newPos == curPos)
        {
            isMoving = false;
            yield break;
        }
        GetComponent<Rigidbody>().freezeRotation = false;
        body.transform.LookAt(newPos);
        GetComponent<Rigidbody>().freezeRotation = true;
        Collider[] obstacles = Physics.OverlapBox(newPos, new Vector3(gridSize/2, .5f, gridSize/2), Quaternion.identity, collisionLayer);
        if(obstacles.Length != 0)
        {
            newPos = curPos;
        }        

        do
        {
            transform.position = Vector3.MoveTowards(curPos, newPos, moveSpeed * Time.deltaTime);
            curPos = transform.position;
            yield return null;
        }while(curPos != newPos);

        Collider[] grass = Physics.OverlapBox(curPos, new Vector3(gridSize/4, .5f, gridSize/4), Quaternion.identity, spawner);
        if(grass.Length != 0 && UnityEngine.Random.Range(0, 100) < encounterOdds)
        {
            yield return new WaitForSeconds(0.5f);
            OnEncounter(GetShinyRolls(), 0, 0);   //Shiny, HA, Perfect IVs
        }        
        isMoving = false;
    }

    // void generateEncounter()
    // {
    //     PokemonSpecies pokemon = determineSpecies();
    //     bool isShiny = determineShiny();

    //     if(isShiny)
    //     {
    //         shinySparkle.Play();
    //     }
    //     else
    //     {                       
    //         shinySparkle.Stop();
    //     }
    // }

    // PokemonSpecies determineSpecies()
    // {
    //     return null;
    // }

    // bool determineShiny()
    // {
    //     int numRolls = getShinyRolls();

    //     for(int i = 0; i < numRolls; i++)
    //     {
    //         if(UnityEngine.Random.Range(0, baseShinyOdds) == 0)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

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
