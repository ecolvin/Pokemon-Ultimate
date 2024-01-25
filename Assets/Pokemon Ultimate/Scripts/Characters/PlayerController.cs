using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform body;    
    [SerializeField] Sprite sprite;
    public Sprite Sprite {get => sprite;}

    [Tooltip("Reference to a shiny sparkle ParticleSystem")]
    [SerializeField] ParticleSystem shinySparkle;

    public event Action<Pokemon> OnEncounter;
    public event Action<Collider> OnTrainerBattle;

    private Character character;
    public Character Character {get => character;}

    bool isMoving = false;

    void Awake()
    {
        character = GetComponent<Character>();
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public void HandleUpdate()
    {
        MovePlayer();    
        if(!isMoving && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))   
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
            //if trainer battle
            //OnTrainerBattle?.Invoke(interactables[0]);
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


        Collider[] triggers = Physics.OverlapBox(transform.position, new Vector3(GlobalSettings.Instance.GridSize/4, .5f, GlobalSettings.Instance.GridSize/4), Quaternion.identity, GameLayers.Instance.TriggerLayers);
        
        bool waitForTriggers = false;

        foreach(var trig in triggers)
        {
            IPlayerTrigger trigger = trig.GetComponent<IPlayerTrigger>();
            if(trigger != null)
            {
                trigger.OnPlayerTriggered(this);
                waitForTriggers = true;
                break;
            }
        }      

        if(!waitForTriggers)
        {
            isMoving = false;
        }
    }

    public void EndTrigger()
    {
        isMoving = false;
    }


    public IEnumerator TriggerEncounter(Pokemon p)
    {
        yield return new WaitForSeconds(0.5f);
        //Screen Flash (and music start eventually)
        OnEncounter(p);   //Shiny, HA, Perfect IVs
        isMoving = false;
    }
}
