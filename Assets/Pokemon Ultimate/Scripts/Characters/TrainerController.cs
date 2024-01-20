using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainerState {Idle, Walking, Dialog, Battle}

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string trainerName;
    public string TrainerName {get => trainerName;}
    [SerializeField] Sprite sprite;
    public Sprite Sprite {get => sprite;}
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog lossDialog;
    public Dialog LossDialog{get => lossDialog;}
    [SerializeField] Dialog postDialog;
    [SerializeField] Transform body;
    [SerializeField] GameObject challenge;

    [SerializeField] [Range(100,9999)] int moveChance = 5000; // 2/moveChance odds per frame
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float patternDelay;

    Character character;
    Party party;
    public Party Party{get => party;}

    TrainerState state = TrainerState.Idle;
    int currentPattern = 0;

    void Awake()
    {
        challenge.SetActive(true);
        character = GetComponent<Character>();
        party = GetComponent<Party>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if(state == TrainerState.Idle)
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
        state = TrainerState.Walking;
        
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
            state = TrainerState.Idle;
            yield break;
        }

        yield return character.SmoothGridMovement(newPos);
        
        if(state != TrainerState.Battle && state != TrainerState.Dialog)
        {
            state = TrainerState.Idle;
        }
    }

    //Figure out why NPC can move through the player
    IEnumerator ScriptedMovement()
    {
        state = TrainerState.Walking;

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

        if(state != TrainerState.Battle && state != TrainerState.Dialog)
        {
            state = TrainerState.Idle;
        }
    }

    //IEnumerator RandomRotation(){}

    public void Interact(Vector3 playerPos)
    {
        body.transform.LookAt(playerPos);
        if(challenge.activeSelf)
        {            
            state = TrainerState.Dialog;
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => 
            {
                state = TrainerState.Battle;
                GameController.Instance.StartTrainerBattle(this);
                challenge.SetActive(false);
            })); 
        }
        else
        {
            state = TrainerState.Dialog;
            StartCoroutine(DialogManager.Instance.ShowDialog(postDialog, () => {state = TrainerState.Idle;}));
        }
    }
}
