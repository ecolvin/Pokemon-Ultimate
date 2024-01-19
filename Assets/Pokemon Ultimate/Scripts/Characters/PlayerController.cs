using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform body;    
    
    [Tooltip("Reference to a shiny sparkle ParticleSystem")]
    [SerializeField] ParticleSystem shinySparkle;      
    [Tooltip("The chance of a grass patch producing an encounter when you walk into it (percentage)")]
    [SerializeField] [Range(0,100)] int encounterOdds = 15;    
    [Tooltip("The base chance of an encounter being a shiny pokemon")]
    [SerializeField] int baseShinyOdds = 4096;

    public event Action<int, int, int> OnEncounter;

    private Character character;

    bool isMoving = false;

    void Awake()
    {
        character = GetComponent<Character>();
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public void HandleUpdate()
    {
        MovePlayer();    
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))   
        {
            Interact();
        } 
    }

    void MovePlayer()
    {
        if(!isMoving)
        {
            StartCoroutine(Move());
        }
    }

    Vector3 getPosLookingAt()
    {
        Vector3 curPos = transform.position;
        int faceDir = (int)body.transform.rotation.eulerAngles.y;
        Vector3 targetPos = curPos;
        if(faceDir == 0)
        {
            targetPos = targetPos + new Vector3(0, 0, 1*GlobalSettings.Instance.GridSize);
        }
        else if(faceDir == 90)
        {
            targetPos = targetPos + new Vector3(1*GlobalSettings.Instance.GridSize, 0, 0);
        }
        else if(faceDir == 180)
        {
            targetPos = targetPos + new Vector3(0, 0, -1*GlobalSettings.Instance.GridSize);
        }
        else if(faceDir == 270)
        {
            targetPos = targetPos + new Vector3(-1*GlobalSettings.Instance.GridSize, 0, 1);
        }
        return targetPos;
    }

    void Interact()
    {
        Vector3 targetPos = getPosLookingAt();

        Collider[] interactables = Physics.OverlapBox(targetPos, new Vector3(GlobalSettings.Instance.GridSize/2, .5f, GlobalSettings.Instance.GridSize/2), Quaternion.identity, GameLayers.Instance.InteractableLayer);
        if(interactables.Length != 0)
        {
            interactables[0].GetComponent<Interactable>()?.Interact(transform.position);
        } 
    }

    IEnumerator Move()
    {        
        isMoving = true;
        
        Vector3 curPos = transform.position;
        float xOffset = Input.GetAxisRaw("Horizontal") * GlobalSettings.Instance.GridSize;
        float zOffset = Input.GetAxisRaw("Vertical") * GlobalSettings.Instance.GridSize;
        Vector3 newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
        if(newPos == curPos)
        {
            isMoving = false;
            yield break;
        }

        yield return character.SmoothGridMovement(newPos);


        Collider[] grass = Physics.OverlapBox(transform.position, new Vector3(GlobalSettings.Instance.GridSize/4, .5f, GlobalSettings.Instance.GridSize/4), Quaternion.identity, GameLayers.Instance.SpawnerLayer);
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
