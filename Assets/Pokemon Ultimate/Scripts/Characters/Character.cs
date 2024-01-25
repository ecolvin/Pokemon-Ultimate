using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{    
    [SerializeField] Transform body;
  
    [SerializeField] float moveSpeed = 50f;

    void Awake()
    {
        SnapToPos(transform.position);
    }

    public void SnapToPos(Vector3 pos)
    {
        pos.x = RoundToNearestTen(pos.x);
        pos.y = 0;
        pos.z = RoundToNearestTen(pos.z);

        transform.position = pos;
    }

    int RoundToNearestTen(float f)
    {
        float r = Mathf.Abs(f % 10);
        Debug.Log($"F = {f}; R = {r}");
        if(r >= 5)
        {
            return Mathf.RoundToInt(f+(10-r));
        }
        else
        {
            return Mathf.RoundToInt(f-r);
        }
    }

    public IEnumerator SmoothGridMovement(Vector3 newPos)
    {       
        Vector3 curPos = transform.position;

        GetComponent<Rigidbody>().freezeRotation = false;
        body.transform.LookAt(newPos);
        GetComponent<Rigidbody>().freezeRotation = true;
        Collider[] obstacles = Physics.OverlapBox(newPos, new Vector3(GlobalSettings.Instance.GridSize/2, 5f, GlobalSettings.Instance.GridSize/2), Quaternion.identity, GameLayers.Instance.BlockingLayers);
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
