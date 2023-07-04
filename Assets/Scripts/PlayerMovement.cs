using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float rotationSpeed = 100f;

    float xSpeed = 0;
    float zSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xSpeed = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        zSpeed = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        movePlayer();
        //rotatePlayer();         
    }

    void movePlayer()
    {
        
        transform.Translate(xSpeed, 0f, zSpeed);
    }

    void rotatePlayer()
    {
        float angle = 0;
        if(xSpeed != 0)
        {
            angle = (float) Math.Atan(zSpeed/xSpeed);
        }
        float rotation = angle * rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0f, rotation, 0f);   
    }
}
