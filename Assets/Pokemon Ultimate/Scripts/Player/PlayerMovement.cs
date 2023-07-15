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

    Vector3 curPos;
    Vector3 newPos;

    float xOffset = 0;
    float zOffset = 0;

    void Start()
    {
        curPos = transform.position;
        newPos = transform.position;
    }

    void Update()
    {
        movePlayer();        
    }

    void movePlayer()
    {
        curPos = transform.position;
        
        if(curPos == newPos) //Not Moving
        {
            xOffset = Input.GetAxisRaw("Horizontal") * gridSize;
            zOffset = Input.GetAxisRaw("Vertical") * gridSize;
            newPos = new Vector3(curPos.x + xOffset, curPos.y, curPos.z + zOffset);
            body.transform.LookAt(newPos);
            Collider[] obstacles = Physics.OverlapBox(newPos, new Vector3(gridSize/2, .5f, gridSize/2), Quaternion.identity, collisionLayer);
            if(obstacles.Length != 0)
            {
                newPos = curPos;
            }        
        }
        transform.position = Vector3.MoveTowards(curPos, newPos, moveSpeed * Time.deltaTime);
    }
}
