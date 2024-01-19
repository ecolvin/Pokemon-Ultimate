using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{    
    [SerializeField] Transform body;
  
    [SerializeField] float moveSpeed = 50f;

    public IEnumerator SmoothGridMovement(Vector3 newPos)
    {       
        Vector3 curPos = transform.position;

        GetComponent<Rigidbody>().freezeRotation = false;
        body.transform.LookAt(newPos);
        GetComponent<Rigidbody>().freezeRotation = true;
        Collider[] obstacles = Physics.OverlapBox(newPos, new Vector3(GlobalSettings.Instance.GridSize/2, 5f, GlobalSettings.Instance.GridSize/2), Quaternion.identity, GameLayers.Instance.ObstacleLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer);
        if(obstacles.Length != 0)
        {
            yield break;
        }        

        do
        {
            transform.position = Vector3.MoveTowards(curPos, newPos, moveSpeed * Time.deltaTime);
            curPos = transform.position;
            yield return null;
        }while(curPos != newPos);
    }
}
