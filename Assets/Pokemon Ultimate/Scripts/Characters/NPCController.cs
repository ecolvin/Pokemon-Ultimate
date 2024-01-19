using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState {Idle, Walking, Dialog}

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Transform body;

    [SerializeField] [Range(100,9999)] int moveChance = 5000; // 2/moveChance odds per frame
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float patternDelay;


    Character character;

    NPCState state = NPCState.Idle;
    int currentPattern = 0;

    void Awake()
    {
        character = GetComponent<Character>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if(state == NPCState.Idle)
        {
            if(movementPattern.Count > 0)
            {
                StartCoroutine(ScriptedMovement());
            }
            else
            {
                StartCoroutine(RandomMovement());
            }
        }
    }

    IEnumerator RandomMovement()
    {
        state = NPCState.Walking;
        
        Vector3 curPos = transform.position;
        float xOffset = 0;
        float zOffset = 0;
        int rand1 = UnityEngine.Random.Range(0,moveChance);
        int rand2 = UnityEngine.Random.Range(0,moveChance);
        if(rand1 <= 1)
        {
            int direction = 1;
            if(rand1 % 2 == 0)
            {
                direction = -1;
            }
            xOffset = direction * GlobalSettings.Instance.GridSize;
        }
        if(rand2 <= 1)
        {
            int direction = 1;
            if(rand2 % 2 == 0)
            {
                direction = -1;
            }
            zOffset = direction * GlobalSettings.Instance.GridSize;
        }
        Vector3 newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
        if(newPos == curPos)
        {
            state = NPCState.Idle;
            yield break;
        }

        yield return character.SmoothGridMovement(newPos);
        
        if(state != NPCState.Dialog)
        {
            state = NPCState.Idle;
        }
    }

    //Figure out why NPC can move through the player
    IEnumerator ScriptedMovement()
    {
        state = NPCState.Walking;

        Vector3 curPos = transform.position;
        Vector2 offset = movementPattern[currentPattern];
        float xOffset = offset.x * GlobalSettings.Instance.GridSize;
        float zOffset = offset.y * GlobalSettings.Instance.GridSize;
        Vector3 newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
        Vector3 prevPos = curPos;
    
        yield return character.SmoothGridMovement(newPos);
        yield return new WaitForSeconds(patternDelay);

        if(transform.position != prevPos && (xOffset != 0 || zOffset != 0))
        {
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        }

        if(state != NPCState.Dialog)
        {
            state = NPCState.Idle;
        }
    }

    //IEnumerator RandomRotation(){}

    public void Interact(Vector3 playerPos)
    {
        state = NPCState.Dialog;
        body.transform.LookAt(playerPos);
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {state = NPCState.Idle;}));      
    }
}
