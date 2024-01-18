using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Transform body;

    [SerializeField] [Range(100,9999)] int moveChance = 5000; // 2/moveChance odds per frame

    Character character;

    bool isMoving = false;

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
        if(!isMoving)
        {
            StartCoroutine(RandomMovement());
        }
    }

    IEnumerator RandomMovement()
    {
        isMoving = true;
        
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
            isMoving = false;
            yield break;
        }

        yield return character.SmoothGridMovement(newPos);

        isMoving = false;
    }

    //IEnumerator ScriptedMovement(){}

    //IEnumerator RandomRotation(){}

    public void Interact(Vector3 playerPos)
    {        
        body.transform.LookAt(playerPos);
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
