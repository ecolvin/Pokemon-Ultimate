using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum NPCState {Idle, Walking, Dialog}

public class NPCController : MonoBehaviour, Interactable, ISaveable
{
    [SerializeField] string id;

    [Header("Basic Components")]
    [SerializeField] Dialog dialog;
    [SerializeField] Transform body;

    [Header("Quest")]
    [SerializeField] List<QuestAction> questActions;

    [Header("Movement")]
    [SerializeField] [Range(100,9999)] int moveOdds = 2500; //Rand(0,n); 0 or 1 to succeed
    [SerializeField] [Range(100,9999)] int rotateOdds = 2500; //Rand(0,n); 0 or 1 to succeed
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float patternDelay;
    [SerializeField] bool onlyRotate;
    [SerializeField] bool lockMovement;

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
            if(onlyRotate)
            {
                RandomRotation();
            }
            else if(movementPattern.Count > 0)
            {
                StartCoroutine(ScriptedMovement());
            }
            else if(!lockMovement)
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
        int rand1 = UnityEngine.Random.Range(0,2*moveOdds);
        int rand2 = UnityEngine.Random.Range(0,2*moveOdds);
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

    void RandomRotation()
    {
        int rand = UnityEngine.Random.Range(0,3 * rotateOdds);
        if(rand <= 2)
        {
            if(rand % 3 == 0)
            {
                body.transform.Rotate(new Vector3(0, 90));
            }
            else if(rand % 3 == 1)
            {
                body.transform.Rotate(new Vector3(0, 180));
            }
            else
            {
                body.transform.Rotate(new Vector3(0, -90));
            }
        }
    }

    public IEnumerator Interact(PlayerController player)
    {
        state = NPCState.Dialog;
        body.transform.LookAt(player.transform.position);

        QuestList questList = player.GetComponent<QuestList>();
        foreach(QuestAction action in questActions)
        {
            int stage = questList.GetStage(action.Quest);
            if(stage == action.Stage)
            {
                yield return action.PerformAction(player);
                state = NPCState.Idle; 
                yield break;   //If quest performed, don't show main dialog
            }
        }

        yield return DialogManager.Instance.ShowDialog(dialog);    
        state = NPCState.Idle;      
    }

    [ContextMenu("Generate guid for id")]
    void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data)
    {
    }

    public void SaveData(ref GameData data)
    {
    }
}
//Implement limited rotation (specific directions only)


